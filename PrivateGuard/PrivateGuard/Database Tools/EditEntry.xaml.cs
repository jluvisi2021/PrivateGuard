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
    public partial class EditEntry : Window
    {


        public EntryObject entry { set;  get; }

        
        public EditEntry(EntryObject obj)
        {
            InitializeComponent();
            
            entry = obj;
            UsernameField.Text = entry.Username.ToString();
            PasswordField.Text = entry.Password.ToString();
            NotesField.Text = entry.Notes.ToString();
            EditTitle.Content = "Editing... ID: " + entry.ID;
            
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {

            EntryObject obj = new EntryObject(entry.ID, UsernameField.Text, PasswordField.Text, NotesField.Text);
            entry = obj;
            Close();
        }
    }
}
