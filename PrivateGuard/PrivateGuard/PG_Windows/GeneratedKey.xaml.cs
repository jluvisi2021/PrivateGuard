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

namespace PrivateGuard.PG_Windows
{
    /// <summary>
    /// Interaction logic for GeneratedKey.xaml
    /// </summary>
    public partial class GeneratedKey : Window
    {
        private readonly Random rand = new Random();
        public GeneratedKey(int num)
        {
            InitializeComponent();
            String gen = "";
            char[] PasswordGenOptionals = "abcdefghikjlmnopqrstuvwxyz1234567890!@#$%^&*()[]{}:'>?/.<,ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            for (int i = 0; i < num; i++)
            {
                
                gen += PasswordGenOptionals[rand.Next(0, PasswordGenOptionals.Length-1)];
            }
            SecureKeyTextBox.Text = gen;
        }
            private void CopyKey_Click(object sender, RoutedEventArgs e)
            {
                MessageBox.Show("Secure key copied to clipboard. \n(CTRL+V to Paste)", "Secure Key", MessageBoxButton.OK, MessageBoxImage.Information);
                Clipboard.SetText(SecureKeyTextBox.Text);
                Close();
            }
        }
    }

