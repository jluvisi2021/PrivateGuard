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
    /// Interaction logic for SaveAsNewKey.xaml
    /// </summary>
    public partial class SaveAsNewKey : Window
    {
        public SaveAsNewKey()
        {
            InitializeComponent();
        }

        public string NewKey { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.IsFileKeyValid(FileKeyField.Text))
            {
                FileKeyField.Text = "";
                MessageBox.Show("File key not valid! Please meet the requirments listed.", "File Key Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                NewKey = FileKeyField.Text;
                Close();
            }
        }

        private void Generate_Secure_Key_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            String gen = "";
            char[] PasswordGenOptionals = "abcdefghikjlmnopqrstuvwxyz1234567890!@#$%^&*()[]{}:'>?/.<,ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            for (int i = 0; i < rand.Next(8, 32); i++)
            {

                gen += PasswordGenOptionals[rand.Next(0, PasswordGenOptionals.Length - 1)];
            }
            FileKeyField.Text = gen;
        }
    }
}
