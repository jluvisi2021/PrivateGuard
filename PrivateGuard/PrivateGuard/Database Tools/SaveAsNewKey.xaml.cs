using System;
using System.Windows;

namespace PrivateGuard.Database_Tools
{
    /// <summary>
    ///     Interaction logic for SaveAsNewKey.xaml
    /// </summary>
    public partial class SaveAsNewKey
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
                MessageBox.Show("File key not valid! Please meet the requirements listed.", "File Key Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                NewKey = FileKeyField.Text;
                Close();
            }
        }

        private void Generate_Secure_Key_Click(object sender, RoutedEventArgs e)
        {
            var rand = new Random();
            var gen = "";
            var passwordGenOptionals =
                "abcdefghikjlmnopqrstuvwxyz1234567890!@#$%^&*()[]{}:'>?/.<,ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            for (var i = 0; i < rand.Next(8, 32); i++)
                gen += passwordGenOptionals[rand.Next(0, passwordGenOptionals.Length - 1)];
            FileKeyField.Text = gen;
        }
    }
}