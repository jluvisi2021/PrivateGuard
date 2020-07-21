using PrivateGuard.PG_Windows;
using System;
using System.Windows;

namespace PrivateGuard.Database_Tools
{
    /// <summary>
    ///     Interaction logic for AddEntry.xaml
    /// </summary>
    public partial class AddEntry
    {
        public EntryObject NewEntryObject { set; get; }

        private readonly int _rowCount;

        public AddEntry(int rowCount)
        {
            _rowCount = rowCount;
            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameField.Text) || string.IsNullOrEmpty(PasswordField.Text))
            {
                MessageBox.Show("You cannot leave the username field or password field empty!", "Error while saving.",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var obj = new EntryObject(_rowCount, UsernameField.Text, PasswordField.Text,
                DateTime.Today.ToShortDateString(), NotesField.Text);
            NewEntryObject = obj;
            Close();
        }
    }
}