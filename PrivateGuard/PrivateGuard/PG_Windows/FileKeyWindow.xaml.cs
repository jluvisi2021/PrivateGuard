using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using PrivateGuard.PG_Data;

namespace PrivateGuard.PG_Windows
{
    /// <summary>
    ///     Interaction logic for FileKeyWindow.xaml
    /// </summary>
    public partial class FileKeyWindow
    {
        private string Key { get; set; }

        public FileKeyWindow()
        {
            InitializeComponent();
            Title = "Create your file key.";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.IsFileKeyValid(FileKeyTextBlock.Text))
            {
                FileKeyTextBlock.Text = "";
                MessageBox.Show("File key not valid! Please meet the requirments listed.", "File Key Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Key = FileKeyTextBlock.Text;
                MessageBox.Show("File key accepted. Please choose a location to save the password database file.",
                    "Save file.", MessageBoxButton.OK, MessageBoxImage.Information);


                var sfd = new SaveFileDialog
                {
                    Filter = "PGM Files (*.pgm)|*.pgm",
                    FilterIndex = 1,
                    FileName = "MyManager",
                    RestoreDirectory = true
                };
                var result = sfd.ShowDialog();

                if (result != true) return;

                File.WriteAllText(sfd.FileName, string.Empty);
                var fs = new FileStream(sfd.FileName, FileMode.Open);
                var bw = new BinaryWriter(fs);
                bw.Write(Cipher.Encrypt("OKAY_TO_ACCESS_MODIFIER_VALUE", Key));
                bw.Write(Environment.NewLine);
                bw.Close();
                fs.Close();
            }
            Close();
        }

    

        private readonly Random rand = new Random();

        private void GenerateSecureKeyButton_Click(object sender, RoutedEventArgs e)
        {
            var gen = "";
            var PasswordGenOptionals =
                "abcdefghikjlmnopqrstuvwxyz1234567890!@#$%^&*()[]{}:'>?/.<,ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            for (var i = 0; i < rand.Next(8, 32); i++)
                gen += PasswordGenOptionals[rand.Next(0, PasswordGenOptionals.Length - 1)];
            FileKeyTextBlock.Text = gen;
        }
    }
}