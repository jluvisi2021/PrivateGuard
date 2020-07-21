using System;
using System.Windows;
using PrivateGuard.PG_Windows;

namespace PrivateGuard.Database_Tools
{
    /// <summary>
    ///     Interaction logic for AddEntry.xaml
    /// </summary>
    public partial class EditEntry
    {
        public EntryObject Entry { set; get; }


        public EditEntry(EntryObject obj)
        {
            InitializeComponent();

            Entry = obj;
            UsernameField.Text = Entry.Username;
            PasswordField.Text = Entry.Password;
            NotesField.Text = Entry.Notes;
            EditTitle.Content = "Editing... ID: " + Entry.ID;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameField.Text) || string.IsNullOrEmpty(PasswordField.Text))
            {
                MessageBox.Show("You cannot leave the username field or password field empty!", "Error while saving.",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var obj = new EntryObject(Entry.ID, UsernameField.Text, PasswordField.Text,
                DateTime.Today.ToShortDateString(), NotesField.Text);
            Entry = obj;
            Close();
        }
    }
}