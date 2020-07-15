﻿using Microsoft.VisualBasic;
using Microsoft.Win32;
using PrivateGuard.PG_Data;
using PrivateGuard.PG_Windows;
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
        public static float VersionID = 0.2F;

        public void TestRWBinaryFile()
        {
            String key = "8UHjPgXZzXCGkhxV2QCnooyJexUzvJrO";


            FileStream fs = new FileStream("C:\\Users\\jluvi\\Desktop\\test.pgm", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            
            bw.Write(Cipher.Encrypt("Hello World! (2)", key));
         
            bw.Close();
            fs.Close();
            FileStream fs2 = new FileStream("C:\\Users\\jluvi\\Desktop\\test.pgm", FileMode.Open);
            BinaryReader bw2 = new BinaryReader(fs2);

            string data = Cipher.Decrypt(bw2.ReadString(), key);
            //string data = bw2.ReadString();
            SelectedFileField.Text = data;

        }
        public MainWindow()
        {

            
            InitializeComponent();
            SetupPrimaryScreen();
           
            
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
            OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
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
                modifier_value = Cipher.Decrypt(bw2.ReadString(), FileKeyField.Text);

            }
            catch (Exception)
            {
                DisplayErrorMessage(ERROR_TYPES.WRONG_CREDENTIALS);
                fs2.Close();
                bw2.Close();
                return;
            }
            //string data = bw2.ReadString();
            if(modifier_value != "OKAY_TO_ACCESS_MODIFIER_VALUE")
            {
                DisplayErrorMessage(ERROR_TYPES.WRONG_CREDENTIALS);
                fs2.Close();
                bw2.Close();
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
        public static bool IsFileKeyValid(String str)
        {

            if(str.Length > 4 && str.Length < 256 && !str.Contains(" ") && !string.IsNullOrWhiteSpace(str))
            {
                return true;
            }
            return false;
        }
        
        private void NewFileButton_Click(object sender, RoutedEventArgs e)
        {
            FileKeyWindow win = new FileKeyWindow();
            win.Show();
        }

        private void Click_for_Source_Code_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/jluvisi2021/PrivateGuard");
        }

        private void Click_for_Source_Code_MouseEnter(object sender, MouseEventArgs e)
        {
            Click_for_Source_Code.Foreground = new SolidColorBrush(Color.FromRgb(63, 128, 232));
        }

        private void Click_for_Source_Code_MouseLeave(object sender, MouseEventArgs e)
        {
            Click_for_Source_Code.Foreground = new SolidColorBrush(Color.FromRgb(187, 192, 195));
        }
    }
}