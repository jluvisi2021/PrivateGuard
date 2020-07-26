
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Exception = System.Exception;

namespace PrivateGuard.Database_Tools
{
    /// <summary>
    /// Interaction logic for ColorTheme.xaml
    /// </summary>
    public partial class ColorTheme
    {
        public static Color FontColor;
        public static Color DatabaseColor;
        /*
         * Check if TextFields are over 3 characters.
         * check if user does not input number.
         * check settings file for color on start
         */
        public ColorTheme()
        {
            InitializeComponent();

            var data = File.ReadAllText(MainWindow.SETTINGS_DIR);
            var rawSettingsData = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            var fRed = byte.Parse(rawSettingsData[8]);
            var fGreen = byte.Parse(rawSettingsData[9]);
            var fBlue = byte.Parse(rawSettingsData[10]);
            var dRed = byte.Parse(rawSettingsData[11]);
            var dGreen = byte.Parse(rawSettingsData[12]);
            var dBlue = byte.Parse(rawSettingsData[13]);

            RedValueFont.Text = string.Empty+fRed;
            GreenValueFont.Text = string.Empty + fGreen;
            BlueValueFont.Text = string.Empty + fBlue;

            RedValueDatabase.Text = string.Empty + dRed;
            GreenValueDatabase.Text = string.Empty + dGreen;
            BlueValueDatabase.Text = string.Empty + dBlue;
            ChangeGlobalFont(App.Font);
            UpdateText();

            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            byte fRed;
            byte fGreen;
            byte fBlue;
            byte dRed;
            byte dGreen;
            byte dBlue;
            try
            {
                fRed = byte.Parse(RedValueFont.Text);
                fGreen = byte.Parse(GreenValueFont.Text);
                fBlue = byte.Parse(BlueValueFont.Text);
                dRed = byte.Parse(RedValueDatabase.Text);
                dGreen = byte.Parse(GreenValueDatabase.Text);
                dBlue = byte.Parse(BlueValueDatabase.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Your values are not input correctly.\nValues must not contain any non numeric characters.\nValues must all be below 255 and above 0.", "Error setting colors.", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FontColor.R = fRed;
            FontColor.G = fGreen;
            FontColor.B = fBlue;

            DatabaseColor.R = dRed;
            DatabaseColor.G = dGreen;
            DatabaseColor.B = dBlue;

            MessageBox.Show("Colors have been changed globally.\nYou may now close this window.", "Colors Changed",
                MessageBoxButton.OK, MessageBoxImage.Information);
            
            App.FontColor.R = fRed;
            App.FontColor.G = fGreen;
            App.FontColor.B = fBlue;
            App.DatabaseColor.R = dRed;
            App.DatabaseColor.G = dGreen;
            App.DatabaseColor.B = dBlue;

            var data = File.ReadAllText(MainWindow.SETTINGS_DIR);
            var b = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            b[8] = string.Empty+fRed;
            b[9] = string.Empty + fGreen;
            b[10] = string.Empty + fBlue;
            b[11] = string.Empty + dRed;
            b[12] = string.Empty + dGreen;
            b[13] = string.Empty + dBlue;
            var newText = string.Empty;
            foreach (var s in b) newText += s + Environment.NewLine;
            File.WriteAllText(MainWindow.SETTINGS_DIR, newText);

        }

        private void ResetDatabaseToDefault_Click(object sender, RoutedEventArgs e)
        {
            RedValueDatabase.Text = "240";
            GreenValueDatabase.Text = "240";
            BlueValueDatabase.Text = "240";
            UpdateText();
        }

        private void ResetFontToDefault_Click(object sender, RoutedEventArgs e)
        {
            RedValueFont.Text = "0";
            GreenValueFont.Text = "0";
            BlueValueFont.Text = "0";
            UpdateText();
        }

        private void IncreaseRedFontButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int red = byte.Parse(RedValueFont.Text);
                if (red == 255)
                {
                    return;
                }
                red++;
                RedValueFont.Text = string.Empty + red;
                UpdateText();
            }
            catch (Exception)
            { }
        }

        private void DecreaseRedFontButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int red = byte.Parse(RedValueFont.Text);
                if (red == 0)
                {
                    return;
                }
                red--;
                RedValueFont.Text = string.Empty + red;
                UpdateText();
            }
            catch (Exception)
            { }
        }

        private void IncreaseGreenFontButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int green = byte.Parse(GreenValueFont.Text);
                if (green == 255)
                {
                    return;
                }
                green++;
                GreenValueFont.Text = string.Empty + green;
                UpdateText();
            }
            catch (Exception)
            { }
            
        }

        private void DecreaseGreenFontButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int green = byte.Parse(GreenValueFont.Text);
                if (green == 0)
                {
                    return;
                }
                green--;
                GreenValueFont.Text = string.Empty + green;
                UpdateText();
            }
            catch (Exception)
            { }
            
        }

        private void IncreaseBlueFontButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int blue = byte.Parse(BlueValueFont.Text);
                if (blue == 255)
                {
                    return;
                }
                blue++;
                BlueValueFont.Text = string.Empty + blue;
                UpdateText();
            }
            catch (Exception)
            { }
            
        }

        private void DecreaseBlueFontButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int blue = byte.Parse(BlueValueFont.Text);
                if (blue == 0)
                {
                    return;
                }
                blue--;
                BlueValueFont.Text = string.Empty + blue;
                UpdateText();
            }
            catch (Exception)
            { }
            
        }

        private void IncreaseRedDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int red = byte.Parse(RedValueDatabase.Text);
                if (red == 255)
                {
                    return;
                }
                red++;
                RedValueDatabase.Text = string.Empty + red;
                UpdateText();
            }
            catch (Exception)
            { }

            
        }

        private void DecreaseRedDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int red = byte.Parse(RedValueDatabase.Text);
                if (red == 0)
                {
                    return;
                }
                red--;
                RedValueDatabase.Text = string.Empty + red;
                UpdateText();
            }
            catch (Exception)
            { }
            
        }

        private void IncreaseGreenDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int green = byte.Parse(GreenValueDatabase.Text);
                if (green == 255)
                {
                    return;
                }
                green++;
                GreenValueDatabase.Text = string.Empty + green;
                UpdateText();
            }
            catch (Exception)
            { }
            
        }

        private void DecreaseGreenDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int green = byte.Parse(GreenValueDatabase.Text);
                if (green == 0)
                {
                    return;
                }
                green--;
                GreenValueDatabase.Text = string.Empty + green;
                UpdateText();
            }
            catch (Exception)
            { }
            
        }

        private void IncreaseBlueDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int blue = byte.Parse(BlueValueDatabase.Text);
                if (blue == 255)
                {
                    return;
                }
                blue++;
                BlueValueDatabase.Text = string.Empty + blue;
                UpdateText();
            }
            catch (Exception)
            { }
            
        }

        private void DecreaseBlueDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int blue = byte.Parse(BlueValueDatabase.Text);
                if (blue == 0)
                {
                    return;
                }
                blue--;
                BlueValueDatabase.Text = string.Empty + blue;
                UpdateText();
            }
            catch (Exception)
            { }
            
        }

        private void RedValueFont_KeyDown(object sender, KeyEventArgs e)
        {

            if (!IsKeyADigit(e.Key) && e.Key != Key.Back || Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                e.Handled = true;
                return;
            }
            if (((TextBox)sender).Text.Length == 3)
            {
                e.Handled = true;
                return;
            }
            UpdateText();
        }

        private void GreenValueFont_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsKeyADigit(e.Key) && e.Key != Key.Back || Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                e.Handled = true;
                return;
            }
            if (((TextBox)sender).Text.Length == 3)
            {
                e.Handled = true;
                return;
            }
            UpdateText();

        }

        private void BlueValueFont_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsKeyADigit(e.Key) && e.Key != Key.Back || Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                e.Handled = true;
                return;
            }
            if (((TextBox)sender).Text.Length == 3)
            {
                e.Handled = true;
                return;
            }
            UpdateText();

        }

        private void RedValueDatabase_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsKeyADigit(e.Key) && e.Key != Key.Back || Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                e.Handled = true;
                return;
            }
            if (((TextBox)sender).Text.Length == 3)
            {
                e.Handled = true;
                return;
            }
            UpdateText();

        }

        private void GreenValueDatabase_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsKeyADigit(e.Key) && e.Key != Key.Back || Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                e.Handled = true;
                return;
            }
            if (((TextBox)sender).Text.Length == 3)
            {
                e.Handled = true;
                return;
            }
            UpdateText();

        }

        private void BlueValueDatabase_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsKeyADigit(e.Key) && e.Key != Key.Back || Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                e.Handled = true;
                return;
            }
            if (((TextBox)sender).Text.Length == 3)
            {
                e.Handled = true;
                return;
            }
            UpdateText();

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

        private void UpdateText()
        {
            byte fRed;
            byte fGreen;
            byte fBlue;
            byte dRed;
            byte dGreen;
            byte dBlue;
            try
            {
                fRed = byte.Parse(RedValueFont.Text);
                fGreen = byte.Parse(GreenValueFont.Text);
                fBlue = byte.Parse(BlueValueFont.Text);
                dRed = byte.Parse(RedValueDatabase.Text);
                dGreen = byte.Parse(GreenValueDatabase.Text);
                dBlue = byte.Parse(BlueValueDatabase.Text);
            }
            catch (Exception)
            {
                return;
            }

            Color testFontColor = Color.FromRgb(fRed, fGreen, fBlue);
            ExampleText1.Foreground = new SolidColorBrush(testFontColor);
            ExampleText2.Foreground = new SolidColorBrush(testFontColor);
            ExampleText3.Foreground = new SolidColorBrush(testFontColor);
            Color testDatabaseColor = Color.FromRgb(dRed, dGreen, dBlue);
            ExampleText4.Foreground = new SolidColorBrush(testDatabaseColor);
            ExampleText5.Foreground = new SolidColorBrush(testDatabaseColor);
            ExampleText6.Foreground = new SolidColorBrush(testDatabaseColor);
           
        }
        public static bool IsKeyADigit(Key key)
        {
            return (key >= Key.D0 && key <= Key.D9) || (key >= Key.NumPad0 && key <= Key.NumPad9);
        }
    }
}
