using PrivateGuard.PG_Windows;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
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

            if (PasswordStrengthBar.Value < 15)
            {
                MessageBox.Show(
                    "Your password has been detected as being weak!\nIt is heavily recommended you attempt to make your password stronger.",
                    "Password Strength", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            var obj = new EntryObject(_rowCount, UsernameField.Text, PasswordField.Text,
                DateTime.Today.ToShortDateString(), NotesField.Text);
            NewEntryObject = obj;
            Close();
        }

        private void PasswordField_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var strength = 0;
            for (var i = 0; i < PasswordField.Text.Length; i++)
            {
                if (char.IsNumber(PasswordField.Text.ToCharArray()[i]))
                {
                    strength += 4;
                }else if (char.IsPunctuation(PasswordField.Text.ToCharArray()[i]))
                {
                    strength += 5;
                }else if(char.IsUpper(PasswordField.Text.ToCharArray()[i]))

                {
                    strength += 3;
                }
                else
                {
                    strength += 2;
                }
                if (PasswordField.Text.Contains(PasswordField.Text.ToCharArray()[i].ToString()))
                {
                    var count = PasswordField.Text.Count(f => f == PasswordField.Text.ToCharArray()[i]);
                    strength -= ((count / 2) - PasswordField.Text.Length / 16);
                }
            }

            PasswordStrengthBar.Value = strength;
           
        }

        private void PasswordField_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var strength = 0;
            for (var i = 0; i < PasswordField.Text.Length; i++)
            {
                if (char.IsNumber(PasswordField.Text.ToCharArray()[i]))
                {
                    strength += 4;
                }
                else if (char.IsPunctuation(PasswordField.Text.ToCharArray()[i]))
                {
                    strength += 5;
                }
                else if (char.IsUpper(PasswordField.Text.ToCharArray()[i]))

                {
                    strength += 3;
                }
                else
                {
                    strength += 2;
                }
                if (PasswordField.Text.Contains(PasswordField.Text.ToCharArray()[i].ToString()))
                {
                    var count = PasswordField.Text.Count(f => f == PasswordField.Text.ToCharArray()[i]);
                    strength -= ((count / 2)-PasswordField.Text.Length/16);
                }
            }
            PasswordStrengthBar.Value = strength;
           
        }
    }
}