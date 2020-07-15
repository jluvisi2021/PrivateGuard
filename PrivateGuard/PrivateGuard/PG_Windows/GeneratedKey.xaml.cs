using System;
using System.Windows;

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

                gen += PasswordGenOptionals[rand.Next(0, PasswordGenOptionals.Length - 1)];
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

