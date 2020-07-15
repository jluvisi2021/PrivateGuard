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
    /// Interaction logic for Database.xaml
    /// </summary>
    /// 

    //TODO: Have trebuchet checked in fonts by default.
    // dont allow users to select more than one font.
    public partial class Database : Window
    {
        private string filename { get; set; }
        public Database(string filename)
        {
            InitializeComponent();
            this.filename = filename;

            EditingLabel.Content = "Editing: " + this.filename.Trim().Split('\\')[this.filename.Trim().Split('\\').Length-1];
        }

        private void ExitProgramLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Exit PrivateGuard?", "Exit", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
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

        private void MinimizeProgramLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MinimizeProgramLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            Label lbl = sender as Label;
            lbl.Foreground = new SolidColorBrush(Color.FromRgb(184, 186, 189));
        }

        private void MinimizeProgramLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            Label lbl = sender as Label;
            lbl.Foreground = new SolidColorBrush(Color.FromRgb(31, 31, 33));
        }



        private void Menu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void EditingLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
