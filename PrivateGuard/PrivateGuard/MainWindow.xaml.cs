﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using PrivateGuard.PG_Data;
using PrivateGuard.PG_Windows;

/*
██████╗░██████╗░██╗██╗░░░██╗░█████╗░████████╗███████╗  ░██████╗░██╗░░░██╗░█████╗░██████╗░██████╗░
██╔══██╗██╔══██╗██║██║░░░██║██╔══██╗╚══██╔══╝██╔════╝  ██╔════╝░██║░░░██║██╔══██╗██╔══██╗██╔══██╗
██████╔╝██████╔╝██║╚██╗░██╔╝███████║░░░██║░░░█████╗░░  ██║░░██╗░██║░░░██║███████║██████╔╝██║░░██║
██╔═══╝░██╔══██╗██║░╚████╔╝░██╔══██║░░░██║░░░██╔══╝░░  ██║░░╚██╗██║░░░██║██╔══██║██╔══██╗██║░░██║
██║░░░░░██║░░██║██║░░╚██╔╝░░██║░░██║░░░██║░░░███████╗  ╚██████╔╝╚██████╔╝██║░░██║██║░░██║██████╔╝
╚═╝░░░░░╚═╝░░╚═╝╚═╝░░░╚═╝░░░╚═╝░░╚═╝░░░╚═╝░░░╚══════╝  ░╚═════╝░░╚═════╝░╚═╝░░╚═╝╚═╝░░╚═╝╚═════╝░

Created by Jacob Luvisi
 */

namespace PrivateGuard

{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _showFileKeyField;
        public static string VersionID = "1.2.1";

        public static readonly string SETTINGS_DIR =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PrivateGuard\\settings.bin";


        public MainWindow()
        {
            InitializeComponent();

            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            // Access settings.bin file.
            if (!File.Exists(appdata + "\\PrivateGuard")) Directory.CreateDirectory(appdata + "\\PrivateGuard");
            if (!File.Exists(appdata + "\\PrivateGuard\\settings.bin"))
            {
                string[] testing =
                {
                    $"Private Guard Settings File: (V.{VersionID})",
                    $"Created On: {DateTime.Now}",
                    "",
                    "IDLE_TIMER: Disabled",
                    "GLOBAL_FONT: Trebuchet MS",
                    "FONT_SIZE: 12px",
                    "AUTO_SAVE: Enabled",
                    "Custom Color Data: (RGB)",
                    "0",
                    "0",
                    "0",
                    "240",
                    "240",
                    "240",
                    "Dark Theme: Disabled"
                };
                File.WriteAllLines(appdata + "\\PrivateGuard\\settings.bin", testing);
            }
            VersionLabel.Content = "Version: " + VersionID;
        }

        private void ExitProgramLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Exit PrivateGuard?", "Exit", MessageBoxButton.OKCancel, MessageBoxImage.Question) ==
                MessageBoxResult.OK) Close();
        }

        private void ExitProgramLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Label) sender).Foreground = new SolidColorBrush(Color.FromRgb(184, 186, 189));
        }


        private void ExitProgramLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Label) sender).Foreground = new SolidColorBrush(Color.FromRgb(31, 31, 33));
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (InvalidOperationException)
            { // Ignore wrong mouse click.
            }
        }

        private void ToggleViewButton_Click(object sender, RoutedEventArgs e)
        {
            _showFileKeyField = !_showFileKeyField;
            ToggleViewButton.Content =
                _showFileKeyField ? ToggleViewButton.Content = "Hide" : ToggleViewButton.Content = "Show";
            if (_showFileKeyField)
            {
                FileKeyField.Visibility = Visibility.Hidden;
                ViewPasswordTextBox.Visibility = Visibility.Visible;
                ViewPasswordTextBox.Text = FileKeyField.Password;
            }
            else
            {
                FileKeyField.Visibility = Visibility.Visible;
                ViewPasswordTextBox.Visibility = Visibility.Hidden;
                FileKeyField.Password = ViewPasswordTextBox.Text;
            }
        }


        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            // Open Window Selector and find .txt file (or binary file) to open.
            var dlg = new OpenFileDialog
            {
                DefaultExt = ".pgm",
                Filter = "PGM Files (*.pgm)|*.pgm"
            };
            var result = dlg.ShowDialog();
            if (result == false) return;

            var fileName = dlg.FileName;
            SelectedFileField.Text = fileName;
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var a = string.Empty;

            var checkable = _showFileKeyField ? ViewPasswordTextBox.Text : FileKeyField.Password;


            if (string.IsNullOrWhiteSpace(FileKeyField.Password))
            {
                DisplayErrorMessage(ErrorTypes.NO_FILE_KEY);
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedFileField.Text))
            {
                DisplayErrorMessage(ErrorTypes.NO_FILE_SELECTED);
                return;
            }

            a = SelectedFileField.Text;
            try
            {
                FileStream fs2 = null;
                BinaryReader bw2 = null;
                try
                {
                    fs2 = new FileStream(SelectedFileField.Text, FileMode.Open);
                    bw2 = new BinaryReader(fs2);
                }
                catch (DirectoryNotFoundException)
                {
                    DisplayErrorMessage(ErrorTypes.FILE_NOT_FOUND);
                    return;
                }
                
                string modifierValue;
                try
                {
                    modifierValue = Cipher.Decrypt(bw2.ReadString(), checkable);
                }
                catch (Exception)
                {
                    DisplayErrorMessage(ErrorTypes.WRONG_CREDENTIALS);
                    fs2.Close();
                    bw2.Close();
                    return;
                }

                if (modifierValue != "OKAY_TO_ACCESS_MODIFIER_VALUE")
                {
                    DisplayErrorMessage(ErrorTypes.WRONG_CREDENTIALS);
                    fs2.Close();
                    bw2.Close();
                    return;
                }

                fs2.Close();
                bw2.Close();

                Database db = new Database(a, checkable);

                if (new FileInfo(SelectedFileField.Text).Length > 500000)
                    MessageBox.Show(
                        "Your file has exceeded the recommended file size limit (500KB).\nAlthough there is no limit to the file size in Private Guard, you may experience slow downs when trying to perform certain actions!",
                        "File Size Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                db.Show();
                Close();
            }
            catch (FileNotFoundException)
            {
                DisplayErrorMessage(ErrorTypes.FILE_NOT_FOUND);
            }
        }


        /// <summary>
        ///     Display a message to the user.
        /// </summary>
        /// <param name="e"></param>
        private static void DisplayErrorMessage(ErrorTypes e)
        {
            switch (e)
            {
                case ErrorTypes.WRONG_CREDENTIALS:
                    MessageBox.Show(
                        "Incorrect Credentials. Please validate that the username and filekey are set properly.",
                        "Error logging in.", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case ErrorTypes.NO_FILE_SELECTED:
                    MessageBox.Show("No file selected. Please select a file using the \"Select\" button.",
                        "Error logging in.", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case ErrorTypes.NO_FILE_KEY:
                    MessageBox.Show(
                        "No file key found. Please fill out the file key text box with the respective file key for the selected file.",
                        "Error logging in.", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case ErrorTypes.FILE_NOT_FOUND:
                    MessageBox.Show("Could not find the file specified! Does it exist?", "Error logging in.",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }

        private enum ErrorTypes
        {
            WRONG_CREDENTIALS,
            NO_FILE_SELECTED,
            NO_FILE_KEY,
            FILE_NOT_FOUND
        }

        private void DocumentationButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/jluvisi2021/PrivateGuard/wiki");
        }

        public static bool IsFileKeyValid(string str)
        {
            if (str.Length > 4 && str.Length < 256 && !str.Contains(" ") && !string.IsNullOrWhiteSpace(str))
                return true;
            return false;
        }

        private void NewFileButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new FileKeyWindow();
            win.Show();
        }

        private void Click_for_Source_Code_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://github.com/jluvisi2021/PrivateGuard");
        }

        private void Click_for_Source_Code_MouseEnter(object sender, MouseEventArgs e)
        {
            Click_for_Source_Code.Foreground = new SolidColorBrush(Color.FromRgb(63, 128, 232));
        }

        private void Click_for_Source_Code_MouseLeave(object sender, MouseEventArgs e)
        {
            Click_for_Source_Code.Foreground = new SolidColorBrush(Color.FromRgb(187, 192, 195));
        }

        private void MinimizeProgramLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MinimizeProgramLabel_MouseEnter(object sender, MouseEventArgs e) =>
            ((Label)sender).Foreground = new SolidColorBrush(Color.FromRgb(184, 186, 189));
        

        private void MinimizeProgramLabel_MouseLeave(object sender, MouseEventArgs e) =>
            ((Label)sender).Foreground = new SolidColorBrush(Color.FromRgb(31, 31, 33));
    }
}