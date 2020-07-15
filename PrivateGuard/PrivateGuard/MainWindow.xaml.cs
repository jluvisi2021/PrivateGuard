using Microsoft.Win32;
using PrivateGuard.PG_Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrivateGuard
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string username = "jluvisi";

        bool ShowFileKeyField = false;
        public static float VersionID = 0.1F;

        public void TestRWBinaryFile()
        {
            String key = "8UHjPgXZzXCGkhxV2QCnooyJexUzvJrO";


            FileStream fs = new FileStream("C:\\Users\\jluvi\\Desktop\\test.pgm", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            
            bw.Write(AES.Encrypt("Hello World! (2)", key));
         
            bw.Close();
            fs.Close();
            FileStream fs2 = new FileStream("C:\\Users\\jluvi\\Desktop\\test.pgm", FileMode.Open);
            BinaryReader bw2 = new BinaryReader(fs2);

            string data = AES.Decrypt(bw2.ReadString(), key);
            //string data = bw2.ReadString();
            SelectedFileField.Text = data;

        }
        public MainWindow()
        {

            
            InitializeComponent();
            SetupPrimaryScreen();
            TestRWBinaryFile();
        }

        /// <summary>
        /// Organizes the primary screen through code rather than XAML.
        /// </summary>
        void SetupPrimaryScreen()
        {
            MainWindow mw = this;
            //mw.Title = "Private Guard | Version: " + VersionID;
            mw.VersionLabel.Content = "Version: " + VersionID;
            //mw.WindowStyle = new WindowStyle {  }; // Make blank top bar.
            OpenFileButton.BorderThickness = new Thickness(2.5);
            NewFileButton.BorderThickness = new Thickness(2.5);
            DocumentationButton.BorderThickness = new Thickness(2.5);
        }

        private void ExitProgramLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(MessageBox.Show("Exit PrivateGuard?", "Exit", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                Close();
            }
            
        }

        private void ExitProgramLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            Label ExitLabel = sender as Label;
            ExitLabel.Foreground = new SolidColorBrush(Color.FromRgb(184, 186, 189));
        }

        private void ExitProgramLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            Label ExitLabel = sender as Label;
            ExitLabel.Foreground = new SolidColorBrush(Color.FromRgb(31, 31, 33));
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove(); // Allows the dragging of the application.
        }

        private void ToggleViewButton_Click(object sender, RoutedEventArgs e)
        {
            ShowFileKeyField = !ShowFileKeyField;
            ToggleViewButton.Content = ShowFileKeyField ? ToggleViewButton.Content = "Hide" : ToggleViewButton.Content = "Show";
        }


        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            // Open Window Selector and find .txt file (or binary file) to open.
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".pgm";
            dlg.Filter = "PGM Files (*.pgm)|*.pgm";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string fileName = dlg.FileName;
                SelectedFileField.Text = fileName;
            }
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            if(UsernameField.Text != username)
            {
                DisplayErrorMessage(ERROR_TYPES.WRONG_CREDENTIALS);
                return;
            }
            if(string.IsNullOrWhiteSpace(FileKeyField.Text))
            {
                DisplayErrorMessage(ERROR_TYPES.NO_FILE_KEY);
                return;
            }
            if(string.IsNullOrWhiteSpace(SelectedFileField.Text))
            {
                DisplayErrorMessage(ERROR_TYPES.NO_FILE_SELECTED);
                return;
            }
            FileStream fs2 = new FileStream(SelectedFileField.Text, FileMode.Open);
            BinaryReader bw2 = new BinaryReader(fs2);
            string modifier_value;
            try
            {
                modifier_value = AES.Decrypt(bw2.ReadString(), FileKeyField.Text);

            }
            catch (EncoderFallbackException)
            {
                DisplayErrorMessage(ERROR_TYPES.WRONG_CREDENTIALS);
                return;
            }
            //string data = bw2.ReadString();
            if(modifier_value != "OKAY_TO_ACCESS_MODIFIER_VALUE")
            {
                DisplayErrorMessage(ERROR_TYPES.WRONG_CREDENTIALS);
                return;
            }
            fs2.Close();
            bw2.Close();
            MessageBox.Show("ALL GOOD!");
            // All is good attempt to open file.

        }

        /// <summary>
        /// Display a message to the user.
        /// </summary>
        /// <param name="e"></param>
        private static void DisplayErrorMessage(ERROR_TYPES e)
        {
            switch(e)
            {
                case ERROR_TYPES.WRONG_CREDENTIALS:
                    MessageBox.Show("Incorrect Credentials.", "Error logging in.", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case ERROR_TYPES.NO_FILE_SELECTED:
                    MessageBox.Show("No file selected. Please select a file using the \"Select\" button.", "Error logging in.", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case ERROR_TYPES.NO_FILE_KEY:
                    MessageBox.Show("No file key found. Please fill out the file key text box with the respective file key for the selected file.", "Error logging in.", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                default:
                    Console.WriteLine("Could not find error: " + e.ToString());
                    break;
            }
        }

        enum ERROR_TYPES
        {
            WRONG_CREDENTIALS, NO_FILE_SELECTED, NO_FILE_KEY
        }

        private void DocumentationButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NewFileButton_Click(object sender, RoutedEventArgs e)
        {
            String FileKey = "8UHjPgXZzXCGkhxV2QCnooyJexUzvJrO";

            Stream myStream;
            SaveFileDialog sfd = new SaveFileDialog();
            

            sfd.Filter = "PGM Files (*.pgm)|*.pgm";
            sfd.FilterIndex = 1;
            sfd.FileName = "MyManager";
            sfd.RestoreDirectory = true;
            Nullable<bool> result = sfd.ShowDialog();

            if (result==true)
            {
                if ((myStream = sfd.OpenFile()) != null)
                {
                    MessageBox.Show($"Saved file at: {sfd.FileName}! \nTo access and/or edit the file input the file key and then use the \"Open File\" button.","Saved File", MessageBoxButton.OK, MessageBoxImage.Information);
                    myStream.Close();
                    Dispatcher.Invoke(() =>
                    {
                        FileStream fs = new FileStream(sfd.FileName, FileMode.Open);
                        BinaryWriter bw = new BinaryWriter(fs);
                        bw.Write(AES.Encrypt("OKAY_TO_ACCESS_MODIFIER_VALUE", FileKey));

                        bw.Close();
                        fs.Close();
                    });
                   
                    // Code to write the stream goes here.
                    
                    
                }
            }
        }
    }
}
