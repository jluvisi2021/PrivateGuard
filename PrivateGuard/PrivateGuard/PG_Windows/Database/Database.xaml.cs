using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using PrivateGuard.Database_Tools;
using PrivateGuard.PG_Data;
using Exception = System.Exception;

namespace PrivateGuard.PG_Windows
{
    /// <summary>
    /// Interaction logic for Database.xaml
    /// </summary>
    /// 

    public partial class Database
    {
        private int _selectedEntry = -1;
        private string Filename { get; }
        private const int CheckInterval = 60 * 1000;
        private readonly string _privateKey;
        private bool _isIdleTimerEnabled = true;
        private bool _isAutoSaveEnabled = false;
        private readonly CancellationTokenSource _token = new CancellationTokenSource();
        private readonly double[] _mousePosition = new double[2];
        private readonly double[] _mousePositionOld = { 0.0, 0.0 };
        private readonly Timer _timer;

        public Database(string filename, string privateKey)
        {
            InitializeComponent();
            Filename = filename;
            _privateKey = privateKey;
            EditingLabel.Content = "Editing: " + Filename.Trim().Split('\\')[Filename.Trim().Split('\\').Length - 1];

            SetupDataGrid();
            GetSettingsFont(File.ReadAllText(MainWindow.SETTINGS_DIR));
            GetSettingsFontSize();
            SetIdleTimerValue();
            SetAutoSaveValue();
            SetupInputBindings();
            _timer = new Timer(Tick, null, 5000, Timeout.Infinite);
            SetupTheme();
        }

        public void SetupTheme()
        {
            try
            {
                var data = File.ReadAllText(MainWindow.SETTINGS_DIR);
                var rawSettingsData = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                var fRed = byte.Parse(rawSettingsData[8]);
                var fGreen = byte.Parse(rawSettingsData[9]);
                var fBlue = byte.Parse(rawSettingsData[10]);
                var dRed = byte.Parse(rawSettingsData[11]);
                var dGreen = byte.Parse(rawSettingsData[12]);
                var dBlue = byte.Parse(rawSettingsData[13]);

                App.FontColor.R = fRed;
                App.FontColor.G = fGreen;
                App.FontColor.B = fBlue;
                App.DatabaseColor.R = dRed;
                App.DatabaseColor.G = dGreen;
                App.DatabaseColor.B = dBlue;

                App.ChangeGlobalFontColor((Panel)Content);
                ChangeDatabaseColor();

            }
            catch (Exception)
            {
                MessageBox.Show("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\nCRITICAL ERROR STARTING APPLICATION.\nWe have detected that you are running version 1.0.6 or greater.\nWe were unable to find the values for font colors in the local settings file.\nThis may be because you have just upgraded from an older version.\n If this is the case please regenerate your settings file by deleting \"settings.bin\" at " + MainWindow.SETTINGS_DIR + ".\nTo read more visit: https://github.com/jluvisi2021/PrivateGuard/wiki \nThe Application will not start until this issue is solved.\n- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -", "Critical Error.", MessageBoxButton.OK, MessageBoxImage.Stop);
                Close();
            }

        }

        /// <summary>
        /// Reads the settings file and determines if the
        /// database should be using the idle timer.
        /// </summary>
        public void SetIdleTimerValue()
        {
            // Idle timer is on.
            var data = File.ReadAllText(MainWindow.SETTINGS_DIR);
            var rawSettingsData = data.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

            if (rawSettingsData[3].Contains("Enabled"))
            {
                return;
            }
            // Idle Timer not in Settings.bin file. We can indicate the user has turned it off.
            _isIdleTimerEnabled = false;
            DisableIdleTimerItem.Header = "Enable Idle Timer";
        }

        public void SetAutoSaveValue()
        {
            // Idle timer is on.
            var data = File.ReadAllText(MainWindow.SETTINGS_DIR);
            var rawSettingsData = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            if (rawSettingsData[6].Contains("Disabled"))
            {
                return;
            }
            if (new FileInfo(Filename).Length > 500000)
            {
                MessageBox.Show(
                    "Your file has exceeded the maximum recommended size of 500KB, therefore auto save has been turned off! To enable autosave shrink your file.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                _isAutoSaveEnabled = false;
                EnableAutoSave.Header = "Enable Auto Save";
                return;
            }
            _isAutoSaveEnabled = true;
            EnableAutoSave.Header = "Disable Auto Save";
        }


        /// <summary>
        /// Setup keyboard short cuts for each Item Menu
        /// </summary>
        public void SetupInputBindings()
        {
            var exportAsText = new RoutedCommand();
            exportAsText.InputGestures.Add(new KeyGesture(Key.T, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(exportAsText, ExportAsTextItem_Click));

            var save = new RoutedCommand();
            save.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(save, SaveItem_Click));

            var addEntry = new RoutedCommand();
            addEntry.InputGestures.Add(new KeyGesture(Key.E, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(addEntry, AddEntryItem_Click));

            var removeEntry = new RoutedCommand();
            removeEntry.InputGestures.Add(new KeyGesture(Key.X, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(removeEntry, RemoveEntryItem_Click));

            var editEntry = new RoutedCommand();
            editEntry.InputGestures.Add(new KeyGesture(Key.W, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(editEntry, EditEntryItem_Click));

            var duplicateEntry = new RoutedCommand();
            duplicateEntry.InputGestures.Add(new KeyGesture(Key.L, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(duplicateEntry, DuplicateEntryItem_Click));

            var disableIdleTimer = new RoutedCommand();
            disableIdleTimer.InputGestures.Add(new KeyGesture(Key.P, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(disableIdleTimer, DisableIdleTimerItem_Click));
        }

        /// <summary>
        /// Increases the font of the program depending on the current font size
        /// that the program is using.
        ///
        /// Depends on the usage of specific button functions to operate.
        ///
        /// Method bound to CTRL+(+)
        /// </summary>
        /// <param name="currentFontSize"></param>
        public void IncreaseFont(int currentFontSize)
        {
            switch (currentFontSize)
            {
                case 8:
                    PasswordDB.FontSize = 12;
                    TextSize12PXItem_Click(this, null);
                    break;
                case 12:
                    PasswordDB.FontSize = 16;
                    TextSize16PXItem_Click(this, null);
                    break;
                case 16:
                    PasswordDB.FontSize = 16;
                    TextSize16PXItem_Click(this, null);
                    break;
            }
        }

        /// <summary>
        /// Decreases the font of the program depending on the current font size
        /// that the program is using.
        ///
        /// Depends on the usage of specific button functions to operate.
        ///
        /// Method bound to CTRL+(-)
        /// </summary>
        /// <param name="currentFontSize"></param>
        public void DecreaseFont(int currentFontSize)
        {
            switch (currentFontSize)
            {
                case 20:
                    PasswordDB.FontSize = 16;
                    TextSize16PXItem_Click(this, null);
                    break;
                case 16:
                    PasswordDB.FontSize = 12;
                    TextSize12PXItem_Click(this, null);
                    break;
                case 12:
                    PasswordDB.FontSize = 8;
                    TextSize8PXItem_Click(this, null);
                    break;
            }
        }

        /// <summary>
        /// Reads the Font Size from Settings.bin.
        /// The font size is located on the 6th line of the file.
        /// </summary>
        public void GetSettingsFontSize()
        {
            var settingsFileData = File.ReadAllText(MainWindow.SETTINGS_DIR);
            var settingsFileSplit = settingsFileData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            if (settingsFileSplit[5].Contains("8px"))
            {
                PasswordDB.FontSize = 8;
                TextSize8PXItem.IsChecked = true;
            }
            else if (settingsFileSplit[5].Contains("12px"))
            {
                PasswordDB.FontSize = 12;
                TextSize12PXItem.IsChecked = true;
            }
            else if (settingsFileSplit[5].Contains("16px"))
            {
                PasswordDB.FontSize = 16;
                TextSize16PXItem.IsChecked = true;
            }
            else if (settingsFileSplit[5].Contains("20px"))
            {
                PasswordDB.FontSize = 20;
                TextSize20PXItem.IsChecked = true;
            }
            else
            {
                MessageBox.Show(
                    "Could not read font size from settings file.\nThe program will still work but Settings will not be saved.\nIf you continue to get this error then try running the program as administrator or regenerate the settings file (See Utilities -> Help).",
                    "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
                PasswordDB.FontSize = 12;
            }
        }

        public void GetSettingsFont(string raw)
        {
            if (raw.Contains("Times New Roman"))
            {
                ChangeGlobalFont("Times New Roman");
                App.Font = "Times New Roman";
                SubFontTimesNewRomanItem.IsChecked = true;
            }
            else if (raw.Contains("Courier"))
            {
                ChangeGlobalFont("Courier");
                App.Font = "Courier";
                SubFontCourierItem.IsChecked = true;
            }
            else if (raw.Contains("Trebuchet MS"))
            {
                ChangeGlobalFont("Trebuchet MS");
                App.Font = "Trebuchet MS";
                SubFontTrebuchetMSItem.IsChecked = true;
            }
            else if (raw.Contains("Arial"))
            {
                ChangeGlobalFont("Arial");
                App.Font = "Arial";
                SubFontArialItem.IsChecked = true;
            }
            else if (raw.Contains("Calibri"))
            {
                ChangeGlobalFont("Calibri");
                App.Font = "Calibri";
                SubFontCalibriItem.IsChecked = true;
            }
            else
            {
                MessageBox.Show(
                    "Could not read font type from settings file.\nThe program will still work but Settings will not be saved.\nIf you continue to get this error then try running the program as administrator or regenerate the settings file (See Utilities -> Help).",
                    "Critical Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ChangeGlobalFont("Trebuchet MS");
                App.Font = "Trebuchet MS";
                SubFontTrebuchetMSItem.IsChecked = true;
            }
        }

        private int _count = 0;
        private void Tick(object state)
        {
            try
            {
                if (_isAutoSaveEnabled)
                {
                    // Save around every 4 minutes
                    ++_count;

                    if (_count / 50 == 0)
                    {

                        SaveItem_Click(null, null);
                    }

                    _count = 0;
                }
                if (!_isIdleTimerEnabled) return;

                // If the mouse position is not in the exact same spot as the last check...
                if (!_mousePositionOld[0].Equals(_mousePosition[0]) ||
                    !_mousePositionOld[1].Equals(_mousePosition[1])) return;
                // Leave the Database and return to the Main Menu.
                Dispatcher.Invoke(() =>
                {
                    var mw = new MainWindow();
                    mw.Show();
                    Close();
                });

                MessageBox.Show("Idle Timer has expired.\nYou have been returned to the home screen.",
                    "Idle Timer", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    
                
            }
            finally
            {
                _timer?.Change(CheckInterval, Timeout.Infinite);
                Dispatcher.Invoke(() =>
                {
                    _mousePositionOld[0] = Mouse.GetPosition(this).X;
                    _mousePositionOld[1] = Mouse.GetPosition(this).Y;
                });
            }
        }

        public void SetupDataGrid()
        {
            var textColumn = new DataGridTextColumn
            {
                Header = "ID #",
                Binding = new Binding("ID"),
                Width = 72
            };
            PasswordDB.Columns.Add(textColumn);

            textColumn = new DataGridTextColumn
            {
                Header = "Username",
                Width = 170,

                Binding = new Binding("Username")
            };
            PasswordDB.Columns.Add(textColumn);

            textColumn = new DataGridTextColumn
            {
                Header = "Password",
                Width = 170,
                Binding = new Binding("Password")
            };
            PasswordDB.Columns.Add(textColumn);

            textColumn = new DataGridTextColumn
            {
                Header = "Date",
                Width = 100,
                Binding = new Binding("Date")
            };
            PasswordDB.Columns.Add(textColumn);

            textColumn = new DataGridTextColumn
            {
                Header = "Notes",
                MaxWidth = 390,
                Binding = new Binding("Notes")
            };
            PasswordDB.Columns.Add(textColumn);

            // Setup a way to decode the values from the .pgm file.
            var fileBytes = File.ReadAllBytes(Filename);
            var rawData = Encoding.UTF8.GetString(fileBytes);

            // Decrypt something
            // Split string on each of its lines.
            var cipherRawDataText = rawData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            // Make it into a list for ease of access.
            List<string> cipherRawDataTextAsList = cipherRawDataText.ToList();
            // Go through the now split array and parse out each of the values.
            // Use the substring to remove the control characters.
            FetchPlainTextDataParallel(cipherRawDataTextAsList, _privateKey);
            SortColumn(PasswordDB, 0);
        }

        /// <summary>
        /// Sorts each value in data grid by ID.
        /// Sorts them in order because they are put out of order in the file because
        /// of Parallel for loops.
        /// </summary>
        /// <param name="dataGrid"></param>
        /// <param name="columnIndex"></param>
        public void SortColumn(DataGrid dataGrid, int columnIndex)
        {
            var performSortMethod = typeof(DataGrid)
                .GetMethod("PerformSort",
                    BindingFlags.Instance | BindingFlags.NonPublic);

            performSortMethod?.Invoke(dataGrid, new[] { dataGrid.Columns[columnIndex] });
        }

        /// <summary>
        /// Decrypts all data in the cipherRawDataText.
        /// </summary>
        /// <param name="cipherRawDataText"></param>
        /// <param name="key"></param>
        private void FetchPlainTextDataParallel(List<string> cipherRawDataText, String key)
        {
            cipherRawDataText.RemoveAt(0);//Remove modifier value.
            Parallel.For(0, cipherRawDataText.Count, (i, state) =>
            {
                if (i % 5 == 0)
                {
                    try
                    {
                        string ID = Cipher.Decrypt(cipherRawDataText[i].Substring(1, cipherRawDataText[i].Length - 1),
                            _privateKey).Trim();
                        string username =
                            Cipher.Decrypt(cipherRawDataText[i + 1].Substring(1, cipherRawDataText[i + 1].Length - 1),
                                _privateKey);
                        string password =
                            Cipher.Decrypt(cipherRawDataText[i + 2].Substring(1, cipherRawDataText[i + 2].Length - 1),
                                _privateKey);
                        string date = Cipher.Decrypt(cipherRawDataText[i + 3].Substring(1, cipherRawDataText[i + 3].Length - 1),
                            _privateKey);
                        string notes =
                            Cipher.Decrypt(cipherRawDataText[i + 4].Substring(1, cipherRawDataText[i + 4].Length - 1),
                                _privateKey);
                        PasswordDB.Items.Add(new EntryObject(int.Parse(ID), username, password, date, notes));

                    }
                    catch (Exception)
                    {
                        
                    }
                    
                }
            });
        }

        private void ExitProgramLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Exit PrivateGuard?\nWould you like to save your changes?\n(May take some time)", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question) !=
                MessageBoxResult.Yes)
            {
                Close();
                return;
            }
            
            SaveItem_Click(null, null);
            Close();

        }

        private void ExitProgramLabel_MouseEnter(object sender, MouseEventArgs e) =>
            ((Label)sender).Foreground = new SolidColorBrush(Color.FromRgb(184, 186, 189));
        

        private void ExitProgramLabel_MouseLeave(object sender, MouseEventArgs e) =>
                ((Label)sender).Foreground = new SolidColorBrush(Color.FromRgb(31, 31, 33));

        private void MinimizeProgramLabel_MouseDown(object sender, MouseButtonEventArgs e) =>
            WindowState = WindowState.Minimized;
        

        private void MinimizeProgramLabel_MouseEnter(object sender, MouseEventArgs e) =>
                ((Label)sender).Foreground = new SolidColorBrush(Color.FromRgb(184, 184, 189));

        private void MinimizeProgramLabel_MouseLeave(object sender, MouseEventArgs e) =>
                ((Label)sender).Foreground = new SolidColorBrush(Color.FromRgb(31, 31, 33));


        private void Menu_MouseDown(object sender, MouseButtonEventArgs e) =>
            DragMove();
        

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e) =>
            DragMove();
        

        private void EditingLabel_MouseDown(object sender, MouseButtonEventArgs e) =>
            DragMove();
        

        private void AddEntryItem_Click(object sender, RoutedEventArgs e)
        {
            var addEntryWindow = new AddEntry(PasswordDB.Items.Count);
                addEntryWindow.ShowDialog();
                if (addEntryWindow.NewEntryObject == null) return;
                PasswordDB.Items.Add(addEntryWindow.NewEntryObject);
        }

        private void PasswordDB_BeginningEdit(object sender, DataGridBeginningEditEventArgs e) =>
            e.Cancel = true;
        

        // Get the ID when the user selects a new row.
        private void PasswordDB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (EntryObject)PasswordDB.SelectedItem;
            if (selectedItem != null)
                _selectedEntry = selectedItem.ID;
        }

        /// <summary>
        /// Removes the current entry at "Selected Entry"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveEntryItem_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEntry == -1)
            {
                MessageBox.Show("Select a row to remove first.", "Error removing entry.", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
             
            if (MessageBox.Show($"Delete entry at {_selectedEntry}?\nWARNING: (You cannot undo this once you save)",
                "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK)
            {
                return;
            }
            PasswordDB.Items.RemoveAt(_selectedEntry);

            for (var i = 0; i < PasswordDB.Items.Count; i++)
            {
                if (!(PasswordDB.Items[i] is EntryObject entry))
                {
                    return;
                }
                entry.ID = i;
                // Replace the object.
                PasswordDB.Items.RemoveAt(i);
                PasswordDB.Items.Insert(i, entry);
            }
            _selectedEntry = -1; // Reset the selected entry as the selection bar is cancelled.
        }

        private void EditEntryItem_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEntry == -1)
            {
                MessageBox.Show("Select a row to edit first.", "Error editing entry.", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            var a = PasswordDB.Items[_selectedEntry] as EntryObject;
            var entry = new EditEntry(a);
            entry.ShowDialog();
            if (entry.Entry == null) return;
            // Replace the entry.
            PasswordDB.Items.RemoveAt(entry.Entry.ID);
            PasswordDB.Items.Insert(entry.Entry.ID, entry.Entry);
            _selectedEntry = -1; // Reset the selected entry as the selection bar is cancelled.
        }

        /// <summary>
        /// Duplicates the selected entry and adds it to the end of the
        /// entry list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DuplicateEntryItem_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEntry == -1)
            {
                MessageBox.Show("Select a row to copy first.", "Error copying entry.", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (!(PasswordDB.Items[_selectedEntry] is EntryObject obj))
            {
                return;
            }
            var copy = (EntryObject)obj.Clone();
            copy.ID = PasswordDB.Items.Count;
            PasswordDB.Items.Add(copy);
        }

        /// <summary>
        /// Removes all entries in the list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteAllEntriesItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Delete ALL entries in the database?\nWARNING: (You cannot undo this once you save)",
                    "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Warning) ==
                MessageBoxResult.OK) PasswordDB.Items.Clear();
        }

        /// <summary>
        /// Instead of using a single-threaded for loop this method uses the built in
        /// C# Parallel.For to compute the encryption for each line. The writing to
        /// the file still needs to be done single-threaded however so this method
        /// just encrypts the data.
        ///
        /// Each list has its own 5 values in it that contains the ID, Username,
        /// Password, Date, and notes.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private IEnumerable<List<string>> ComputeEncryption(String key)
        {
            var identifierList = new List<List<string>>();
            Parallel.For(0, PasswordDB.Items.Count, i =>
            {
                if (!(PasswordDB.Items[i] is EntryObject entry))
                {
                    return;
                }
                var subList = new List<string>
                {
                    Cipher.Encrypt("" + entry.ID, key),
                    Cipher.Encrypt("" + entry.Username, key),
                    Cipher.Encrypt("" + entry.Password, key),
                    Cipher.Encrypt("" + entry.Date, key),
                    Cipher.Encrypt("" + entry.Notes, key)
                };
                identifierList.Add(subList);
            });
            return identifierList;
        }
        /// <summary>
        /// Saves all entries and writes them to a file using the encryption.
        /// </summary>
        /// <see cref="Cipher"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveItem_Click(object sender, RoutedEventArgs e)
        {
            var filePath = Filename;
            try
            {
                File.WriteAllText(filePath, string.Empty);
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Error while saving. Make sure program has read and write permissions and try again. If this continues to fail try running the program in administrator mode.",
                    "Error while saving", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }
            var fs = new FileStream(Filename, FileMode.Open, FileAccess.Write);
            var bw = new BinaryWriter(fs);
            try
            {
                bw.Write(Cipher.Encrypt("OKAY_TO_ACCESS_MODIFIER_VALUE", _privateKey));
                bw.Write(Environment.NewLine);
                
                foreach(var list in ComputeEncryption(_privateKey))
                {
                    bw.Write(list[0]);
                    bw.Write(Environment.NewLine);
                    bw.Write(list[1]);
                    bw.Write(Environment.NewLine);
                    bw.Write(list[2]);
                    bw.Write(Environment.NewLine);
                    bw.Write(list[3]);
                    bw.Write(Environment.NewLine);
                    bw.Write(list[4]);
                    bw.Write(Environment.NewLine);
                }

            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Error while saving. Make sure program has read and write permissions and try again. If this continues to fail try running the program in administrator mode.",
                    "Error while saving", MessageBoxButton.OK, MessageBoxImage.Error);
                
                return;
            }
            finally
            {
                fs.Close();
                bw.Close();
            }
           
            MessageBox.Show("Database Saved & Encrypted", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Return to Main Menu?", "Exit", MessageBoxButton.OKCancel, MessageBoxImage.Question) !=
                MessageBoxResult.OK)
            {
                return;
            }

            _timer.Dispose();
            var mw = new MainWindow();
            mw.Show();
            Close();
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
            var sfd = new SaveFileDialog
            {
                Filter = "TXT Files (*.txt)|*.txt",
                FilterIndex = 1
            };

            var fileName = Filename.Trim().Split('\\')[Filename.Trim().Split('\\').Length - 1]; // Remove path
            var name = fileName.Substring(0, fileName.Length - 4); // Remove file extension.

            sfd.FileName = name + "_plain";
            sfd.RestoreDirectory = true;
            var result = sfd.ShowDialog();

            if (result == false)
            {
                return;
            }

            myStream = sfd.OpenFile();
            try
            {
                MessageBox.Show(
                    $"Saved plain text passwords file at {sfd.FileName}!\nRemember these passwords are unencrypted so handle them carefully.",
                    "Saved!", MessageBoxButton.OK, MessageBoxImage.Information);
                myStream.Close();
                var a = new List<string>
                {
                    "<-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=->",
                    "[Passwords Plain Text File]",
                    $"Exported from Private Guard Password Manager. (V{MainWindow.VersionID})",
                    $"Exported on: {DateTime.Now}",
                    $"File Key: {_privateKey}",
                    $"Database Size: {new FileInfo(Filename).Length / 1000}KB",
                    "<-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=->",
                    "" // Add new line
                };

                
                foreach (EntryObject entry in PasswordDB.Items)
                {
                    a.Add(entry.ToString());
                }
                

                var lines = a.ToArray();


                File.WriteAllLines(sfd.FileName, lines);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            
            
        }

        /// <summary>
        /// Toggle the idle timer. (on/off)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisableIdleTimerItem_Click(object sender, RoutedEventArgs e)
        {
            _isIdleTimerEnabled = !_isIdleTimerEnabled;
            var menuItem = (MenuItem)e.OriginalSource;
            if (_isIdleTimerEnabled)
            {
                menuItem.Header = "Disable Idle Timer";
                // Idle timer is on.
                var data = File.ReadAllText(MainWindow.SETTINGS_DIR);
                var b = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                b[3] = "IDLE_TIMER: Enabled";
                var newText = string.Empty;
                foreach (var s in b) newText += s + Environment.NewLine;
                File.WriteAllText(MainWindow.SETTINGS_DIR, newText);
            }
            else
            {
                menuItem.Header = "Enable Idle Timer";
                // Idle timer is off.
                var data = File.ReadAllText(MainWindow.SETTINGS_DIR);
                var b = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                b[3] = "IDLE_TIMER: Disabled";
                var newText = string.Empty;
                foreach (var s in b) newText += s + Environment.NewLine;
                File.WriteAllText(MainWindow.SETTINGS_DIR, newText);
            }
        }

        private void GeneratePasswordItem_Click(object sender, RoutedEventArgs e)
        {
            var pg = new PasswordGen();
            pg.Show();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            _mousePosition[0] = Mouse.GetPosition(this).X;
            _mousePosition[1] = Mouse.GetPosition(this).Y;
        }

        //TODO: Make fonts stay with local settings file.


        private void ChangeGlobalFont(string font)
        {
            // casting the content into panel
            var mainContainer = (Panel)Content;

            // GetAll UIElement
            var element = mainContainer.Children;

            // casting the UIElementCollection into List
            var lstElement = element.Cast<FrameworkElement>().ToList();

            // Getting all Control from list
            var lstControl = lstElement.OfType<Control>();

            App.Font = font;
            foreach (var control in lstControl) 
                
                // If the control is not the minimize or exit function.
                if (!control.Name.Contains("Program"))
                    //Hide all Controls
                    control.FontFamily = new FontFamily(font);

        }


        private void SubFontTimesNewRomanItem_Click(object sender, RoutedEventArgs e)
        {
            
            SubFontTimesNewRomanItem.IsChecked = true;
            SubFontArialItem.IsChecked = false;
            SubFontTrebuchetMSItem.IsChecked = false;
            SubFontCourierItem.IsChecked = false;
            SubFontCalibriItem.IsChecked = false;
            ChangeGlobalFont("Times New Roman");

            var data = File.ReadAllText(MainWindow.SETTINGS_DIR);
            var b = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            b[4] = "GLOBAL_FONT: Times New Roman";
            var newText = new StringBuilder();
            foreach (var s in b) newText.Append(s).Append(Environment.NewLine);
            File.WriteAllText(MainWindow.SETTINGS_DIR, newText.ToString());
        }

        private void SubFontArialItem_Click(object sender, RoutedEventArgs e)
        {
            SubFontArialItem.IsChecked = true;
            SubFontTimesNewRomanItem.IsChecked = false;
            SubFontTrebuchetMSItem.IsChecked = false;
            SubFontCourierItem.IsChecked = false;
            SubFontCalibriItem.IsChecked = false;
            ChangeGlobalFont("Arial");
            var data = File.ReadAllText(MainWindow.SETTINGS_DIR);
            var b = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            b[4] = "GLOBAL_FONT: Arial";
            var newText = new StringBuilder();
            foreach (var s in b) newText.Append(s).Append(Environment.NewLine);
            File.WriteAllText(MainWindow.SETTINGS_DIR, newText.ToString());
        }

        private void SubFontTrebuchetMSItem_Click(object sender, RoutedEventArgs e)
        {
            SubFontTrebuchetMSItem.IsChecked = true;
            SubFontArialItem.IsChecked = false;
            SubFontTimesNewRomanItem.IsChecked = false;
            SubFontCourierItem.IsChecked = false;
            SubFontCalibriItem.IsChecked = false;
            ChangeGlobalFont("Trebuchet MS");
            var data = File.ReadAllText(MainWindow.SETTINGS_DIR);
            var b = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            b[4] = "GLOBAL_FONT: Trebuchet MS";
            var newText = new StringBuilder();
            foreach (var s in b) newText.Append(s).Append(Environment.NewLine);
            File.WriteAllText(MainWindow.SETTINGS_DIR, newText.ToString());
        }

        private void SubFontCourierItem_Click(object sender, RoutedEventArgs e)
        {
            SubFontCourierItem.IsChecked = true;
            SubFontArialItem.IsChecked = false;
            SubFontTrebuchetMSItem.IsChecked = false;
            SubFontTimesNewRomanItem.IsChecked = false;
            SubFontCalibriItem.IsChecked = false;
            ChangeGlobalFont("Courier");
            var data = File.ReadAllText(MainWindow.SETTINGS_DIR);
            var b = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            b[4] = "GLOBAL_FONT: Courier";
            var newText = new StringBuilder();
            foreach (var s in b) newText.Append(s).Append(Environment.NewLine);
            File.WriteAllText(MainWindow.SETTINGS_DIR, newText.ToString());
        }

        private void SubFontCalibriItem_Click(object sender, RoutedEventArgs e)
        {
            SubFontArialItem.IsChecked = false;
            SubFontTrebuchetMSItem.IsChecked = false;
            SubFontTimesNewRomanItem.IsChecked = false;
            SubFontCourierItem.IsChecked = false;
            ChangeGlobalFont("Calibri");
            var data = File.ReadAllText(MainWindow.SETTINGS_DIR);
            var b = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            b[4] = "GLOBAL_FONT: Calibri";
            var newText = new StringBuilder();
            foreach (var s in b) newText.Append(s).Append(Environment.NewLine);
            File.WriteAllText(MainWindow.SETTINGS_DIR, newText.ToString());
        }

        private void TextSize8PXItem_Click(object sender, RoutedEventArgs e)
        {
            TextSize8PXItem.IsChecked = true;
            PasswordDB.FontSize = 8;
            var data = File.ReadAllText(MainWindow.SETTINGS_DIR);
            var b = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            b[5] = "FONT_SIZE: 8px";
            var newText = new StringBuilder();
            foreach (var s in b) newText.Append(s).Append(Environment.NewLine);
            File.WriteAllText(MainWindow.SETTINGS_DIR, newText.ToString());
            TextSize12PXItem.IsChecked = false;
            TextSize16PXItem.IsChecked = false;
            TextSize20PXItem.IsChecked = false;
        }

        private void TextSize12PXItem_Click(object sender, RoutedEventArgs e)
        {
            TextSize12PXItem.IsChecked = true;
            PasswordDB.FontSize = 12;
            var data = File.ReadAllText(MainWindow.SETTINGS_DIR);
            var b = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            b[5] = "FONT_SIZE: 12px";
            var newText = new StringBuilder();
            foreach (var s in b) newText.Append(s).Append(Environment.NewLine);
            File.WriteAllText(MainWindow.SETTINGS_DIR, newText.ToString());
            TextSize8PXItem.IsChecked = false;
            TextSize16PXItem.IsChecked = false;
            TextSize20PXItem.IsChecked = false;
        }

        private void TextSize16PXItem_Click(object sender, RoutedEventArgs e)
        {
            TextSize16PXItem.IsChecked = true;
            PasswordDB.FontSize = 16;
            var data = File.ReadAllText(MainWindow.SETTINGS_DIR);
            var b = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            b[5] = "FONT_SIZE: 16px";
            var newText = new StringBuilder();
            foreach (var s in b) newText.Append(s).Append(Environment.NewLine);
            File.WriteAllText(MainWindow.SETTINGS_DIR, newText.ToString());
            TextSize8PXItem.IsChecked = false;
            TextSize12PXItem.IsChecked = false;
            TextSize20PXItem.IsChecked = false;
        }

        private void TextSize20PXItem_Click(object sender, RoutedEventArgs e)
        {
            TextSize20PXItem.IsChecked = true;
            PasswordDB.FontSize = 20;
            var data = File.ReadAllText(MainWindow.SETTINGS_DIR);
            var b = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            b[5] = "FONT_SIZE: 20px";
            var newText = new StringBuilder();
            foreach (var s in b) newText.Append(s).Append(Environment.NewLine);
            File.WriteAllText(MainWindow.SETTINGS_DIR, newText.ToString());
            TextSize8PXItem.IsChecked = false;
            TextSize16PXItem.IsChecked = false;
            TextSize12PXItem.IsChecked = false;
        }

        private void HelpItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/jluvisi2021/PrivateGuard/wiki");
        }

        private void ContactDevItem_Click(object sender, RoutedEventArgs e)
        {
            var c = new Contact();
            c.ShowDialog();
        }

        private void Copy_Password_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEntry == -1)
                return;
            if (!(PasswordDB.Items[_selectedEntry] is EntryObject obj))
            {
                return;
            }
            Clipboard.SetText(obj.Password);
        }

        private void CopyUsername_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEntry == -1)
                return;
            if (!(PasswordDB.Items[_selectedEntry] is EntryObject obj))
            {
                return;
            }
            Clipboard.SetText(obj.Username);
        }

        private void EditEntry_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEntry == -1) return;
            var a = PasswordDB.Items[_selectedEntry] as EntryObject;
            var entry = new EditEntry(a);
            entry.ShowDialog();
            if (entry.Entry == null) return;
            PasswordDB.Items.RemoveAt(entry.Entry.ID);
            PasswordDB.Items.Insert(entry.Entry.ID, entry.Entry);
            _selectedEntry = -1; // Reset the selected entry as the selection bar is cancelled.
        }

        private void RemoveEntry_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEntry == -1) return;
            if (MessageBox.Show($"Delete entry at {_selectedEntry}?\nWARNING: (You cannot undo this once you save)",
                "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK)
            {
                return;
            }

            PasswordDB.Items.RemoveAt(_selectedEntry);

                for (var i = 0; i < PasswordDB.Items.Count; i++)
                {
                if (!(PasswordDB.Items[i] is EntryObject obj))
                {
                    return;
                }
                obj.ID = i;
                    // Replace the object.
                    PasswordDB.Items.RemoveAt(i);
                    PasswordDB.Items.Insert(i, obj);
                }

                _selectedEntry = -1; // Reset the selected entry as the selection bar is cancelled.
            
        }

        private void SaveAsItem_Click(object sender, RoutedEventArgs e)
        {
            var specifiedKey = _privateKey;
            if (MessageBox.Show(
                "Save with a different Encryption Key?\nThis will be the key you use to open this new file.",
                "Encryption Key", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var newKeyWindow = new SaveAsNewKey();
                newKeyWindow.ShowDialog();
                specifiedKey = newKeyWindow.NewKey;
                if (string.IsNullOrWhiteSpace(specifiedKey)) return;
            }

            var sfd = new SaveFileDialog
            {
                Filter = "PGM Files (*.pgm)|*.pgm",
                FilterIndex = 1,
                FileName = "MyManager",
                RestoreDirectory = true
            };
            var result = sfd.ShowDialog();

            if (result != true) return;

            File.WriteAllText(sfd.FileName, string.Empty);
            var fs = new FileStream(sfd.FileName, FileMode.Open);
            var bw = new BinaryWriter(fs);
            bw.Write(Cipher.Encrypt("OKAY_TO_ACCESS_MODIFIER_VALUE", specifiedKey));
            bw.Write(Environment.NewLine);
            // Write each object on a different line
            foreach (List<string> list in ComputeEncryption(specifiedKey))
            {
                bw.Write(list[0]);
                bw.Write(Environment.NewLine);
                bw.Write(list[1]);
                bw.Write(Environment.NewLine);
                bw.Write(list[2]);
                bw.Write(Environment.NewLine);
                bw.Write(list[3]);
                bw.Write(Environment.NewLine);
                bw.Write(list[4]);
                bw.Write(Environment.NewLine);
            }

            MessageBox.Show(
                $"Saved file at: {sfd.FileName}! \nTo access and/or edit the file input the file key and then use the \"Open File\" button.",
                "Saved File", MessageBoxButton.OK, MessageBoxImage.Information);
            
            bw.Close();
            fs.Close();

                    // Code to write the stream goes here.
                
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.OemPlus:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        IncreaseFont((int)PasswordDB.FontSize);
                    break;
                case Key.OemMinus:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        DecreaseFont((int)PasswordDB.FontSize);
                    break;
                default:
                    return;
            }
        }

        private void EnableAutoSave_Click(object sender, RoutedEventArgs e)
        {
            if (new FileInfo(Filename).Length > 500000 && _isAutoSaveEnabled == false)
            {
                MessageBox.Show(
                    "Your file has exceeded the maximum recommended size of 500KB, therefore auto save cannot be enabled! To enable autosave shrink your file.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                return;
            }
            _isAutoSaveEnabled = !_isAutoSaveEnabled;
            var menuItem = (MenuItem)e.OriginalSource;
            if (_isAutoSaveEnabled)
            {
                menuItem.Header = "Disable Auto Save";

                var data = File.ReadAllText(MainWindow.SETTINGS_DIR);
                var b = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                b[6] = "AUTO_SAVE: Enabled";
                var newText = string.Empty;
                foreach (var s in b) newText += s + Environment.NewLine;
                File.WriteAllText(MainWindow.SETTINGS_DIR, newText);
            }
            else
            {
                menuItem.Header = "Enable Auto Save";
                
                var data = File.ReadAllText(MainWindow.SETTINGS_DIR);
                var b = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                b[6] = "AUTO_SAVE: Disabled";
                var newText = string.Empty;
                foreach (var s in b) newText += s + Environment.NewLine;
                File.WriteAllText(MainWindow.SETTINGS_DIR, newText);
            }
        }

        private void ChangeThemeItem_Click(object sender, RoutedEventArgs e)
        {
            // Opens a new window where a user can pick an RGB value for the text and an RGB value for the database background.
            var colorThemeWindow = new ColorTheme();
            colorThemeWindow.ShowDialog();
            ChangeDatabaseColor();
            App.ChangeGlobalFontColor((Panel)Content);
        }
        private void ChangeDatabaseColor()
        {
            PasswordDB.RowBackground = new SolidColorBrush(App.DatabaseColor);
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
            return MemberwiseClone();
        }

        public override string ToString()
        {
            return $"ID: {ID} | Username:{Username} | Password:{Password} | Date: {Date} | Notes: {Notes}";
        }
    }
}