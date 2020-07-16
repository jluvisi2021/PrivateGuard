using PrivateGuard.Database_Tools;
using PrivateGuard.PG_Data;
using System;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
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
        private string filename { get; set; }
        private readonly string PrivateKey;
        public Database(string filename, string privatekey)
        {
            InitializeComponent();
            this.filename = filename;

            EditingLabel.Content = "Editing: " + this.filename.Trim().Split('\\')[this.filename.Trim().Split('\\').Length - 1];
            SetupDataGrid();
            PrivateKey = privatekey;


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
            TextColumn.Header = "Notes";
            //TextColumn.Width = 170;
            TextColumn.MaxWidth = 350;
            TextColumn.Binding = new Binding("Notes");
            PasswordDB.Columns.Add(TextColumn);
            
            // Setup a way to decode the values from the .pgm file.
           
        }

        private void ExitProgramLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Exit PrivateGuard?\nAny unsaved data will be lost.", "Exit", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
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
                if(Entry.entry == null)
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
            if(SelectedEntry == -1)
            {
                MessageBox.Show("Select a row to remove first.", "Error removing entry.", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MessageBox.Show($"Delete entry at {SelectedEntry}?\nWARNING: (You cannot undo this once you save)", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Warning)==MessageBoxResult.OK)
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
            if(Entry.entry == null)
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
            obj.ID = PasswordDB.Items.Count;
            // Deselect all items.
            PasswordDB.SelectedItems.Clear();
            PasswordDB.SelectedCells.Clear();
            PasswordDB.Items.Add(obj);
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
        private readonly string Seperator = "//JexUzvJrO";
        private void SaveItem_Click(object sender, RoutedEventArgs e)
        {
            // Bless up <3
            String FILE_PATH = filename;
            try
            {
                File.WriteAllText(FILE_PATH, String.Empty);
            }
            catch(Exception)
            {
                MessageBox.Show("Error while saving. Make sure program has read and write permissions and try again. If this continues to fail try running the program in administrator mode.", "Error while saving", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            FileStream fs = null;
            BinaryWriter bw = null;
            try
            {
                fs = new FileStream(filename, FileMode.Open);
                bw = new BinaryWriter(fs);
                bw.Write(Cipher.Encrypt("OKAY_TO_ACCESS_MODIFIER_VALUE", PrivateKey));
                bw.Write(Environment.NewLine);
            for (int i = 0; i < PasswordDB.Items.Count; i++)
                {
                    EntryObject temp = PasswordDB.Items[i] as EntryObject;
                    bw.Write("" + i + Seperator + temp.Username + Seperator + temp.Password + Seperator + temp.Notes);
                    bw.Write(Environment.NewLine);
            }
            } catch (Exception)
            {
                MessageBox.Show("Error while saving. Make sure program has read and write permissions and try again. If this continues to fail try running the program in administrator mode.", "Error while saving", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            } finally
            {
                bw.Close();
                fs.Close();
           }
            MessageBox.Show("Database Saved & Encrypted", "Success", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            return;
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Exit PrivateGuard?\nAny unsaved data will be lost.", "Exit", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                Close();
            }
        }

        
        private void CopyPasswordButton_Click(object sender, RoutedEventArgs e)
        {
           
        }


        private void CopyUsernameButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
    public class EntryObject
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Notes { get; set; }

        public EntryObject(int ID, string Username, string Password, string Notes)
        {
            this.ID = ID;
            this.Username = Username;
            this.Password = Password;
            this.Notes = Notes;
        }


    }
}
