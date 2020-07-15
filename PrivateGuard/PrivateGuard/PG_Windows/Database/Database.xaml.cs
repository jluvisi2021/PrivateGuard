using PrivateGuard.Database_Tools;
using System;
using System.Data;
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
        public Database(string filename)
        {
            InitializeComponent();
            this.filename = filename;

            EditingLabel.Content = "Editing: " + this.filename.Trim().Split('\\')[this.filename.Trim().Split('\\').Length - 1];
            SetupDataGrid();


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
            
           
        }

        void TestRows()
        {
            var Entry1 = new EntryObject(0, "jluvisi", "1162", "my account");
            PasswordDB.Items.Add(Entry1);
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
            AddEntry Entry = new AddEntry(PasswordDB.Items.Count);
            Entry.ShowDialog();
            PasswordDB.Items.Add(Entry.entry);
           
            //PasswordDB.Items.Refresh();
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
            if (MessageBox.Show($"Delete entry at {SelectedEntry}? (You cannot undo this)", "Confirm", MessageBoxButton.OKCancel, MessageBoxImage.Warning)==MessageBoxResult.OK)
            {
                PasswordDB.Items.RemoveAt(SelectedEntry);

                for (int i = 0; i < PasswordDB.Items.Count; i++)
                {
                    EntryObject obj = PasswordDB.Items[i] as EntryObject;
                    obj.ID = i;
                    PasswordDB.Items.RemoveAt(i);
                    PasswordDB.Items.Insert(i, obj);
                }
                return;
            }
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
