using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace PrivateGuard.Database_Tools
{
    /// <summary>
    ///     Interaction logic for PasswordGen.xaml
    /// </summary>
    public partial class PasswordGen
    {
        //TODO: Make a numbers only option.
        private readonly Random _random = new Random();
        private readonly char[] _normalChars = "abcdefghiklmnopqrstuvwxyz".ToCharArray();
        private readonly char[] _capitalChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private readonly char[] _specialChars = "!@#$%^&*()[]{}:;<>".ToCharArray();
        private readonly char[] _numericalChars = "123456789".ToCharArray();
        public PasswordGen()
        {
            InitializeComponent();
            PasswordLengthSlider.Minimum = 4;
            PasswordLengthSlider.Maximum = 256;
        }


        private void PasswordLengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PasswordLengthCurrent.Content = (int) e.NewValue;
        }

        private void GeneratePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var password = new StringBuilder();
            var capsEnabled = CapitalLetters.IsChecked ?? true;
            var specialEnabled = SpecialSymbols.IsChecked ?? true;
            var numbersEnabled = UseNumbers.IsChecked ?? true;
            var values = new List<char>(_normalChars);
            if (capsEnabled) values.AddRange(_capitalChars);
            if (specialEnabled) values.AddRange(_specialChars);
            if(numbersEnabled) values.AddRange(_numericalChars);
            for (var i = 0; i < int.Parse(PasswordLengthCurrent.Content.ToString()); i++)
                password.Append(values[_random.Next(0, values.Count)]);
            PasswordBox.Text = password.ToString();
        }
    }
}