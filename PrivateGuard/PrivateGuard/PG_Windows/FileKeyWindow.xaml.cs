using Microsoft.Win32;
using PrivateGuard.PG_Data;
using System;
using System.IO;
using System.Windows;

namespace PrivateGuard.PG_Windows
{
    /// <summary>
    /// Interaction logic for FileKeyWindow.xaml
    /// </summary>
    public partial class FileKeyWindow : Window
    {
        private String Key { get; set; }
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
                MessageBox.Show("File key not valid! Please meet the requirments listed.", "File Key Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Key = FileKeyTextBlock.Text;
                MessageBox.Show("File key accepted. Please choose a location to save the password database file.", "Save file.", MessageBoxButton.OK, MessageBoxImage.Information);


                Stream myStream;
                SaveFileDialog sfd = new SaveFileDialog();


                sfd.Filter = "PGM Files (*.pgm)|*.pgm";
                sfd.FilterIndex = 1;
                sfd.FileName = "MyManager";
                sfd.RestoreDirectory = true;
                Nullable<bool> result = sfd.ShowDialog();

                if (result == true)
                {
                    if ((myStream = sfd.OpenFile()) != null)
                    {
                        MessageBox.Show($"Saved file at: {sfd.FileName}! \nTo access and/or edit the file input the file key and then use the \"Open File\" button.", "Saved File", MessageBoxButton.OK, MessageBoxImage.Information);
                        myStream.Close();


                        FileStream fs = new FileStream(sfd.FileName, FileMode.Open);
                        BinaryWriter bw = new BinaryWriter(fs);
                        bw.Write(Cipher.Encrypt("OKAY_TO_ACCESS_MODIFIER_VALUE", Key));

                        bw.Close();
                        fs.Close();


                        // Code to write the stream goes here.


                    }
                }
                Close();
            }
        }
        readonly Random rand = new Random();
        private void GenerateSecureKeyButton_Click(object sender, RoutedEventArgs e)
        {
            GeneratedKey GenKeyWin = new GeneratedKey(rand.Next(8, 32));
            GenKeyWin.Show();
        }
    }
}
