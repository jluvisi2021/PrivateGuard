using PrivateGuard.PG_Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PrivateGuard.Database_Tools
{
    /// <summary>
    /// Interaction logic for AddEntry.xaml
    /// </summary>
    public partial class AddEntry : Window
    {


        public EntryObject entry { set;  get; }

        readonly int rowcount;
        public AddEntry(int rowcount)
        {
            this.rowcount = rowcount;
            InitializeComponent();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
           if(string.IsNullOrWhiteSpace(UsernameField.Text) || string.IsNullOrEmpty(PasswordField.Text)) {
                MessageBox.Show("You cannot leave the username field or password field empty!", "Error while saving.", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            EntryObject obj = new EntryObject(rowcount, UsernameField.Text, PasswordField.Text, DateTime.Today.ToShortDateString(), NotesField.Text);
            entry = obj;
            Close();
        }
    }
}
