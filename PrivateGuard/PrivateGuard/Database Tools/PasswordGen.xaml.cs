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

namespace PrivateGuard.Database_Tools
{
    /// <summary>
    /// Interaction logic for PasswordGen.xaml
    /// </summary>
    public partial class PasswordGen : Window
    {
        private readonly Random rand = new Random();
        private readonly char[] NormalChars = "abcdefghiklmnopqrstuvwxyz".ToCharArray();
        private readonly char[] CapitalChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private readonly char[] SpecialChars = "!@#$%^&*()[]{}:;<>".ToCharArray();
        public PasswordGen()
        {
            InitializeComponent();
            PasswordLengthSlider.Minimum = 4;
            PasswordLengthSlider.Maximum = 256;
        }

        

        private void PasswordLengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            PasswordLengthCurrent.Content = (int)e.NewValue;
        }

        private void GeneratePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder Password = new StringBuilder();
            bool CapsEnabled = CapitalLetters.IsChecked ?? true;
            bool SpecialEnabled = SpecialSymbols.IsChecked ?? true;
            List<char> values = new List<char>(NormalChars);
            if (CapsEnabled)
            {
                values.AddRange(CapitalChars);
            }
            if (SpecialEnabled)
            {
                values.AddRange(SpecialChars);
            }
            for (int i = 0; i < int.Parse(PasswordLengthCurrent.Content.ToString()); i++)
            {
                Password.Append(values[rand.Next(0, values.Count)]);
            }
            PasswordBox.Text = Password.ToString();
        }
    }
}
