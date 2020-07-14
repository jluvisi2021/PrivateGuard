using System;
using System.Collections.Generic;
using System.Linq;
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
        bool ShowFileKeyField = false;
        public static float VersionID = 0.1F;


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
    }
}
