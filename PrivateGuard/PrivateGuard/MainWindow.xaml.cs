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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrivateGuard
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            mw.Title = "Private Guard | Version: " + VersionID;
            mw.VersionLabel.Content = "Version: " + VersionID;
            
        }
    }
}
