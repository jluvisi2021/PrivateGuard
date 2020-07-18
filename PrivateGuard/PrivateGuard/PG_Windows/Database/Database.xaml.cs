using Microsoft.Win32;
using PrivateGuard.Database_Tools;
using PrivateGuard.PG_Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace PrivateGuard.PG_Windows
{
    /// <summary>
    /// Interaction logic for Database.xaml
    /// </summary>
    /// 

    //TODO: Have trebuchet checked in fonts by default.
    // dont allow users to select more than one font.
    public partial class Database : Window
    {

        int SelectedEntry = -1;
        private string Filename { get; set; }
        private readonly int CheckInterval = 60 * 1000;
        private readonly string PrivateKey;
        private bool IsIdleTimerEnabled = true;
        private readonly CancellationTokenSource token = new CancellationTokenSource();
        private readonly double[] MousePosition = new double[2];
        private readonly double[] MousePositionOld = { 0.0, 0.0 };
        private Timer _timer;
        public Database(string filename, string privatekey)
        {

            InitializeComponent();
            this.Filename = filename;

            EditingLabel.Content = "Editing: " + this.Filename.Trim().Split('\\')[this.Filename.Trim().Split('\\').Length - 1];
            SetupDataGrid();
            PrivateKey = privatekey;
            // Run Idle Timer.
            ManageIdle();
            string Raw_Settings = File.ReadAllText(MainWindow.SETTINGS_DIR);
            // Change the global font.
            GetSettingsFont(Raw_Settings);
            SetIdleTimerValue();
        }

        public void SetIdleTimerValue()
        {
            // Idle timer is on.
            String Data = File.ReadAllText(MainWindow.SETTINGS_DIR);
            string[] b = Data.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            if (!b[3].Contains("Enabled"))
            {
                // Idle timer off.
                IsIdleTimerEnabled = false;
                DisableIdleTimerItem.Header = "Enable Idle Timer";

            }
        }

        public void GetSettingsFont(string raw)
        {
            if (raw.Contains("Times New Roman"))
            {
                ChangeGlobalFont("Times New Roman");
                SubFontTimesNewRomanItem.IsChecked = true;
            }
            else if (raw.Contains("Courier"))
            {
                ChangeGlobalFont("Courier");
                SubFontCourierItem.IsChecked = true;
            }
            else if (raw.Contains("Trebuchet MS"))
            {
                ChangeGlobalFont("Trebuchet MS");
                SubFontTrebuchetMSItem.IsChecked = true;
            }
            else if (raw.Contains("Arial"))
            {
                ChangeGlobalFont("Arial");
                SubFontArialItem.IsChecked = true;
            }
            else if (raw.Contains("Calibri"))
            {
                ChangeGlobalFont("Calibri");
                SubFontCalibriItem.IsChecked = true;
            }
            else
            {
                MessageBox.Show("Could not read font from settings file.\nIf you continue to get this error then try running the program as administrator.", "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ChangeGlobalFont("Trebuchet MS");
                SubFontTrebuchetMSItem.IsChecked = true;
            }
        }

        public void ManageIdle()
        {
            _timer = new Timer(Tick, null, CheckInterval, Timeout.Infinite);
        }

        private void Tick(object state)
        {

            try
            {
                if (IsIdleTimerEnabled)
                {
                    Console.WriteLine("Checking Mouse Position...");
                    if (MousePositionOld[0] == MousePosition[0] && MousePositionOld[1] == MousePosition[1])
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MainWindow mw = new MainWindow();
                            mw.Show();
                            Close();
                        });

                        MessageBox.Show("Idle Timer has expired.\nYou have been returned to the home screen.", "Idle Timer", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                    }
                }
                else
                {
                    Console.WriteLine("Skipped Mouse Check.");
                }
            }
            finally
            {
                _timer?.Change(CheckInterval, Timeout.Infinite);
                Dispatcher.Invoke(() =>
                {
                    MousePositionOld[0] = Mouse.GetPosition(this).X;
                    MousePositionOld[1] = Mouse.GetPosition(this).Y;
                });

            }
        }

        public void SetupDataGrid()
        {

            DataGridTextColumn TextColumn = new DataGridTextColumn();
            TextColumn.Header = "ID #";
            TextColumn.Binding = new Binding("ID");
            TextColumn.Width = 72;
            PasswordDB.Columns.Add(TextColumn);

            TextColumn = new DataGridTextColumn();
            TextColumn.Header = "Username";
            TextColumn.Width = 170;

            TextColumn.Binding = new Binding("Username");
            PasswordDB.Columns.Add(TextColumn);

            TextColumn = new DataGridTextColumn();
            TextColumn.Header = "Password";
            TextColumn.Width = 170;
            TextColumn.Binding = new Binding("Password");
            PasswordDB.Columns.Add(TextColumn);

            TextColumn = new DataGridTextColumn();
            TextColumn.Header = "Date";
            //TextColumn.Width = 170;
            TextColumn.Width = 100;
            TextColumn.Binding = new Binding("Date");
            PasswordDB.Columns.Add(TextColumn);

            TextColumn = new DataGridTextColumn();
            TextColumn.Header = "Notes";
            //TextColumn.Width = 170;
            TextColumn.MaxWidth = 390;
            TextColumn.Binding = new Binding("Notes");
            PasswordDB.Columns.Add(TextColumn);

            // Setup a way to decode the values from the .pgm file.
            byte[] FileBytes = File.ReadAllBytes(Filename);
            string RawData = Encoding.UTF8.GetString(FileBytes);

            // Decrypt something
            // Split string on each of its lines.
            string[] b = RawData.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            // Make it into a list for ease of access.
            List<string> SeperateObjectValues = new List<string>();
            foreach (string s in b)
            {
                if (s.Length == 0)
                {
                    break;
                }
                //MessageBox.Show("s is: " + s + "\nLength:"+s.Length);
                string a = s.Substring(1, s.Length - 1);
                SeperateObjectValues.Add(a);
            }

            SeperateObjectValues.RemoveAt(0); // Remove the header.

            for (int i = 0; i < SeperateObjectValues.Count; i += 5)
            {

                string ID = SeperateObjectValues[i].Substring(0, SeperateObjectValues[i].Length - 1);
                string Username = SeperateObjectValues[i + 1];
                string Password = SeperateObjectValues[i + 2];
                string Date = SeperateObjectValues[i + 3];
                string Notes = SeperateObjectValues[i + 4];
                EntryObject entry = new EntryObject(int.Parse(ID.Trim()), Username, Password, Date, Notes);
                PasswordDB.Items.Add(entry);

            }
        }

        private void ExitProgramLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Exit PrivateGuard?", "Exit", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                Close();
            }
        }

        private void ExitProgramLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            Label ExitLabel = sender as Label;
            ExitLabel.Foreground = new SolidColorBrush(Color.FromRgb(184, 186, 189));
        }

        private void ExitProgramLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            Label ExitLabel = sender as Label;
            ExitLabel.Foreground = new SolidColorBrush(Color.FromRgb(31, 31, 33));
        }

        private void MinimizeProgramLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MinimizeProgramLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            Label lbl = sender as Label;
            lbl.Foreground = new SolidColorBrush(Color.FromRgb(184, 186, 189));
        }

        private void MinimizeProgramLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            Label lbl = sender as Label;
            lbl.Foreground = new SolidColorBrush(Color.FromRgb(31, 31, 33));
        }



        private void Menu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void EditingLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void AddEntryItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddEntry Entry = new AddEntry(PasswordDB.Items.Count);
                Entry.ShowDialog();
                if (Entry.entry == null)
                {
                    return;
                }

                PasswordDB.Items.Add(Entry.entry);

                //PasswordDB.Items.Refresh();
            }
            catch (Exception) { }
        }

        private void PasswordDB_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            //  DataRowView DataRowView = (DataRowView)PasswordDB.SelectedItem;
            //  int RowID = Convert.ToInt32(DataRowView.Row[0]);
            //   SelectedEntry = RowID;
            // var selectedItem = PasswordDB.SelectedItem as EntryObject;
            // if (selectedItem != null)
            //     MessageBox.Show(selectedItem.ID.ToString());
            // MessageBox.Show("" + SelectedEntry);
            e.Cancel = true;
        }

        // Get the ID when the user selects a new row.
        private void PasswordDB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = PasswordDB.SelectedItem as EntryObject;
            if (selectedItem != null)
                SelectedEntry = selectedItem.ID;
        }

        private void RemoveEntryItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEntry == -1)
            {
                MessageBox.Show("Select a row to remove first.", "Error removing entry.", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MessageBox.Show($"Delete entry at {SelectedEntry}?\nWARNING: (You cannot undo this once you save)", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                PasswordDB.Items.RemoveAt(SelectedEntry);

                for (int i = 0; i < PasswordDB.Items.Count; i++)
                {
                    EntryObject obj = PasswordDB.Items[i] as EntryObject;
                    obj.ID = i;
                    // Replace the object.
                    PasswordDB.Items.RemoveAt(i);
                    PasswordDB.Items.Insert(i, obj);
                }
                SelectedEntry = -1; // Reset the selected entry as the selection bar is cancelled.
                return;
            }
        }

        private void EditEntryItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEntry == -1)
            {
                MessageBox.Show("Select a row to edit first.", "Error editing entry.", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            EntryObject a = PasswordDB.Items[SelectedEntry] as EntryObject;
            EditEntry Entry = new EditEntry(a);
            Entry.ShowDialog();
            if (Entry.entry == null)
            {
                return;
            }
            PasswordDB.Items.RemoveAt(Entry.entry.ID);
            PasswordDB.Items.Insert(Entry.entry.ID, Entry.entry);
            SelectedEntry = -1; // Reset the selected entry as the selection bar is cancelled.
            return;
        }

        private void DuplicateEntryItem_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEntry == -1)
            {
                MessageBox.Show("Select a row to copy first.", "Error copying entry.", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            EntryObject obj = PasswordDB.Items[SelectedEntry] as EntryObject;
            EntryObject copy = (EntryObject)obj.Clone();
            copy.ID = PasswordDB.Items.Count;
            // Deselect all items.
            PasswordDB.SelectedItems.Clear();
            PasswordDB.SelectedCells.Clear();
            PasswordDB.Items.Add(copy);
            SelectedEntry = -1;
        }

        private void DeleteAllEntriesItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Delete ALL entries in the database?\nWARNING: (You cannot undo this once you save)", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                PasswordDB.Items.Clear();
            }
            return;
        }
        private void SaveItem_Click(object sender, RoutedEventArgs e)
        {
            // Bless up <3
            String FILE_PATH = Filename;
            FileStream fs = null;
            BinaryWriter bw = null;
            try
            {
                File.WriteAllText(FILE_PATH, String.Empty);
                fs = new FileStream(Filename, FileMode.Open);
                bw = new BinaryWriter(fs);
                bw.Write(Cipher.Encrypt("OKAY_TO_ACCESS_MODIFIER_VALUE", PrivateKey));
                bw.Write(Environment.NewLine);
                for (int i = 0; i < PasswordDB.Items.Count; i++)
                {
                    EntryObject temp = PasswordDB.Items[i] as EntryObject;
                    // Write each value on a seperate line.
                    //bw.Write("" + i + Seperator + temp.Username + Seperator + temp.Password + Seperator + temp.Date + Seperator + temp.Notes);
                    bw.Write(Cipher.Encrypt("" + i, PrivateKey));
                    bw.Write(Environment.NewLine);
                    bw.Write(Cipher.Encrypt(temp.Username, PrivateKey));
                    bw.Write(Environment.NewLine);
                    bw.Write(Cipher.Encrypt(temp.Password, PrivateKey));
                    bw.Write(Environment.NewLine);
                    bw.Write(Cipher.Encrypt(temp.Date, PrivateKey));
                    bw.Write(Environment.NewLine);
                    bw.Write(Cipher.Encrypt(temp.Notes, PrivateKey));
                    bw.Write(Environment.NewLine);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error while saving. Make sure program has read and write permissions and try again. If this continues to fail try running the program in administrator mode.", "Error while saving", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            finally
            {
                bw.Close();
                fs.Close();
            }
            MessageBox.Show("Database Saved & Encrypted", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            return;
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Return to Main Menu?", "Exit", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                MainWindow mw = new MainWindow();
                mw.Show();
                Close();
            }
        }

        /// <summary>
        /// Creates a text file then uses the ToString() methods on all of the objects
        /// currently in the database table.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportAsTextItem_Click(object sender, RoutedEventArgs e)
        {
            Stream myStream;
            SaveFileDialog sfd = new SaveFileDialog();


            sfd.Filter = "TXT Files (*.txt)|*.txt";
            sfd.FilterIndex = 1;
            String _filename = this.Filename.Trim().Split('\\')[this.Filename.Trim().Split('\\').Length - 1]; // Remove path
            String name = _filename.Substring(0, _filename.Length - 4); // Remove file extension.

            sfd.FileName = name + "_plain";
            sfd.RestoreDirectory = true;
            Nullable<bool> result = sfd.ShowDialog();

            if (result == true)
            {
                if ((myStream = sfd.OpenFile()) != null)
                {
                    MessageBox.Show($"Saved plain text passwords file at {sfd.FileName}!\nRemember these passwords are unencrypted so handle them carefully.", "Saved!", MessageBoxButton.OK, MessageBoxImage.Information);
                    myStream.Close();
                    List<string> a = new List<string>();
                    a.Add("<-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=->");
                    a.Add("[Passwords Plain Text File]");
                    a.Add("Exported from Private Guard Password Manager. (V" + MainWindow.VersionID + ")");
                    a.Add("Exported on: " + DateTime.Now.ToString());
                    a.Add("File Key: " + PrivateKey);
                    double length = new FileInfo(Filename).Length;
                    a.Add("Database Size: " + length / 1000 + " MB");
                    a.Add("<-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=->");
                    a.Add(""); // Add new line
                    for (int i = 0; i < PasswordDB.Items.Count; i++)
                    {
                        EntryObject en = PasswordDB.Items[i] as EntryObject;
                        a.Add(en.ToString());
                    }
                    string[] lines = a.ToArray();


                    File.WriteAllLines(sfd.FileName, lines);
                }
            }
        }

        /// <summary>
        /// Toggle the idle timer. (on/off)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisableIdleTimerItem_Click(object sender, RoutedEventArgs e)
        {
            IsIdleTimerEnabled = !IsIdleTimerEnabled;
            var menuItem = (MenuItem)e.OriginalSource;
            if (IsIdleTimerEnabled)
            {
                menuItem.Header = "Disable Idle Timer";
                // Idle timer is on.
                String Data = File.ReadAllText(MainWindow.SETTINGS_DIR);
                string[] b = Data.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                b[3] = "IDLE_TIMER: Enabled";
                string NewText = string.Empty;
                foreach (string s in b)
                {
                    NewText += s + Environment.NewLine;
                }
                File.WriteAllText(MainWindow.SETTINGS_DIR, NewText);
            }
            else
            {
                menuItem.Header = "Enable Idle Timer";
                // Idle timer is off.
                String Data = File.ReadAllText(MainWindow.SETTINGS_DIR);
                string[] b = Data.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                b[3] = "IDLE_TIMER: Disabled";
                string NewText = string.Empty;
                foreach (string s in b)
                {
                    NewText += s + Environment.NewLine;
                }
                File.WriteAllText(MainWindow.SETTINGS_DIR, NewText);
            }

        }

        private void GeneratePasswordItem_Click(object sender, RoutedEventArgs e)
        {
            PasswordGen pg = new PasswordGen();
            pg.Show();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            MousePosition[0] = Mouse.GetPosition(this).X;
            MousePosition[1] = Mouse.GetPosition(this).Y;
            Console.WriteLine(MousePosition[0] + ", " + MousePosition[1]);
            Console.WriteLine(MousePositionOld[0] + ", " + MousePositionOld[1]);
            Console.WriteLine();

        }

        //TODO: Make fonts stay with local settings file.


        private void ChangeGlobalFont(String font)
        {
            /// casting the content into panel
            Panel mainContainer = (Panel)Content;

            /// GetAll UIElement
            UIElementCollection element = mainContainer.Children;

            /// casting the UIElementCollection into List
            List<FrameworkElement> lstElement = element.Cast<FrameworkElement>().ToList();

            /// Geting all Control from list
            var lstControl = lstElement.OfType<Control>();

            foreach (Control contol in lstControl)
            {
                ///Hide all Controls
                contol.FontFamily = new FontFamily(font);
            }
        }

        private void SubFontTimesNewRomanItem_Click(object sender, RoutedEventArgs e)
        {
            SubFontArialItem.IsChecked = false;
            SubFontTrebuchetMSItem.IsChecked = false;
            SubFontCourierItem.IsChecked = false;
            SubFontCalibriItem.IsChecked = false;
            ChangeGlobalFont("Times New Roman");
            String Data = File.ReadAllText(MainWindow.SETTINGS_DIR);
            string[] b = Data.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            b[4] = "GLOBAL_FONT: Times New Roman";
            string NewText = string.Empty;
            foreach (string s in b)
            {
                NewText += s + Environment.NewLine;
            }
            File.WriteAllText(MainWindow.SETTINGS_DIR, NewText);
        }

        private void SubFontArialItem_Click(object sender, RoutedEventArgs e)
        {
            SubFontTimesNewRomanItem.IsChecked = false;
            SubFontTrebuchetMSItem.IsChecked = false;
            SubFontCourierItem.IsChecked = false;
            SubFontCalibriItem.IsChecked = false;
            ChangeGlobalFont("Arial");
            String Data = File.ReadAllText(MainWindow.SETTINGS_DIR);
            string[] b = Data.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            b[4] = "GLOBAL_FONT: Arial";
            string NewText = string.Empty;
            foreach (string s in b)
            {
                NewText += s + Environment.NewLine;
            }
            File.WriteAllText(MainWindow.SETTINGS_DIR, NewText);
        }

        private void SubFontTrebuchetMSItem_Click(object sender, RoutedEventArgs e)
        {
            SubFontArialItem.IsChecked = false;
            SubFontTimesNewRomanItem.IsChecked = false;
            SubFontCourierItem.IsChecked = false;
            SubFontCalibriItem.IsChecked = false;
            ChangeGlobalFont("Trebuchet MS");
            String Data = File.ReadAllText(MainWindow.SETTINGS_DIR);
            string[] b = Data.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            b[4] = "GLOBAL_FONT: Trebuchet MS";
            string NewText = string.Empty;
            foreach (string s in b)
            {
                NewText += s + Environment.NewLine;
            }
            File.WriteAllText(MainWindow.SETTINGS_DIR, NewText);
        }

        private void SubFontCourierItem_Click(object sender, RoutedEventArgs e)
        {
            SubFontArialItem.IsChecked = false;
            SubFontTrebuchetMSItem.IsChecked = false;
            SubFontTimesNewRomanItem.IsChecked = false;
            SubFontCalibriItem.IsChecked = false;
            ChangeGlobalFont("Courier");
            String Data = File.ReadAllText(MainWindow.SETTINGS_DIR);
            string[] b = Data.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            b[4] = "GLOBAL_FONT: Courier";
            string NewText = string.Empty;
            foreach (string s in b)
            {
                NewText += s + Environment.NewLine;
            }
            File.WriteAllText(MainWindow.SETTINGS_DIR, NewText);
        }

        private void SubFontCalibriItem_Click(object sender, RoutedEventArgs e)
        {
            SubFontArialItem.IsChecked = false;
            SubFontTrebuchetMSItem.IsChecked = false;
            SubFontTimesNewRomanItem.IsChecked = false;
            SubFontCourierItem.IsChecked = false;
            ChangeGlobalFont("Calibri");
            String Data = File.ReadAllText(MainWindow.SETTINGS_DIR);
            string[] b = Data.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            b[4] = "GLOBAL_FONT: Calibri";
            string NewText = string.Empty;
            foreach (string s in b)
            {
                NewText += s + Environment.NewLine;
            }
            File.WriteAllText(MainWindow.SETTINGS_DIR, NewText);
        }
    }
    public class EntryObject : ICloneable
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string Date { get; set; }
        public string Notes { get; set; }



        public EntryObject(int ID, string Username, string Password, string Date, string Notes)
        {
            this.ID = ID;
            this.Username = Username;
            this.Password = Password;
            this.Date = Date;
            this.Notes = Notes;
        }

        /// <summary>
        /// Creates a shallow clone of the object in new memory.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override string ToString()
        {
            return $"ID: {this.ID} | Username:{this.Username} | Password:{this.Password} | Date: {this.Date} | Notes: {this.Notes}";
        }


    }
}
