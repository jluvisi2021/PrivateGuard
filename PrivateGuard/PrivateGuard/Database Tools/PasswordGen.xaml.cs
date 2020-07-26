using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            ChangeGlobalFont(App.Font);
            App.ChangeGlobalFontColor((Panel)Content);
        }

        private void ChangeGlobalFont(string font)
        {
            // casting the content into panel
            var mainContainer = (Panel)Content;

            // GetAll UIElement
            var element = mainContainer.Children;

            // casting the UIElementCollection into List
            var lstElement = element.Cast<FrameworkElement>().ToList();

            // Getting all Control from list
            var lstControl = lstElement.OfType<Control>();

            App.Font = font;
            foreach (var control in lstControl)

                // If the control is not the minimize or exit function.
                if (!control.Name.Contains("Program"))
                    //Hide all Controls
                    control.FontFamily = new FontFamily(font);

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