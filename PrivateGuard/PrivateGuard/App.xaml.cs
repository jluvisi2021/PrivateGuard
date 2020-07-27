using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PrivateGuard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App
    {
        public static string Font = "Trebuchet MS";
        public static Color FontColor = Color.FromRgb(0, 0, 0);
        public static Color DatabaseColor = Color.FromRgb(240, 240, 240);
        public static bool DarkThemeEnabled = false;

        public static SolidColorBrush DarkModeBackground = new SolidColorBrush(Color.FromRgb(50, 50, 50));
        public static SolidColorBrush DarkModeSecondaryBackground = new SolidColorBrush(Color.FromRgb(65, 65, 65));
        public static SolidColorBrush DarkModeText = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        public static SolidColorBrush DarkModeHighlightText = new SolidColorBrush(Color.FromRgb(195, 195, 195));

        /// <summary>
        /// Checks if dark mode is enabled and if it is applies the
        /// preset.
        /// </summary>
        /// <param name="Content"></param>
        public static void CheckForDarkMode(Panel Content)
        {
            if (!DarkThemeEnabled)
            {
                return;
            }
            
            // casting the content into panel
            var mainContainer = Content;

            // GetAll UIElement
            var element = mainContainer.Children;

            // casting the UIElementCollection into List
            var lstElement = element.Cast<FrameworkElement>().ToList();

            // Getting all Control from list
            var lstControl = lstElement.OfType<Control>();

            Content.Background = DarkModeBackground;
            foreach (var control in lstControl)

                // If the control is not the minimize or exit function.
                if (!control.Name.Contains("Program"))
                {
                    if (control.GetType() == typeof(TextBox))
                    {
                        control.Background = DarkModeSecondaryBackground;
                        control.Foreground = DarkModeText;
                        ((TextBox) control).SelectionBrush = DarkModeHighlightText;
                    }
                    else if (control.GetType() == typeof(Label))
                    {
                        control.Foreground = DarkModeText;
                    }else if (control.GetType() == typeof(Separator))
                    {
                        control.Foreground = DarkModeText;
                    
                    }else if (control.GetType() == typeof(ProgressBar))
                    {
                        control.Background = DarkModeSecondaryBackground;
                        control.Foreground = DarkModeText;
                    }
                    else if (control.GetType() == typeof(Button))
                    {
                        control.Foreground = DarkModeText;
                        control.Background = DarkModeSecondaryBackground;
                    }else if (control.GetType() == typeof(CheckBox))
                    {
                        control.Foreground = DarkModeText;
                        control.Background = DarkModeSecondaryBackground;
                    }
                    else if (control.GetType() == typeof(Slider))
                    {
                        control.Background = DarkModeSecondaryBackground;
                    }else if (control.GetType() == typeof(DataGrid))
                    {
                        control.Foreground = DarkModeText;
                        control.BorderBrush = DarkModeBackground;
                        control.Background = DarkModeSecondaryBackground;
                        // Apply the visual changes for the specific data grid type.
                        ((DataGrid) control).RowBackground = DarkModeSecondaryBackground;
                        ((DataGrid) control).ColumnHeaderStyle = (Style)Current.FindResource("DarkColumnHeader");
                    }
                    else if (control.GetType() == typeof(Menu))
                    {
                        var menu = (Menu) control;
                        // Sets all of the menu items to the set color.
                        menu.Foreground = DarkModeText;
                        menu.Background = DarkModeSecondaryBackground;
                        menu.BorderBrush = DarkModeHighlightText;
                        foreach (MenuItem item in menu.Items)
                        {
                            foreach (MenuItem subItem in item.Items)
                            {
                                foreach (MenuItem extendedSubMenuItem in subItem.Items)
                                {
                                    extendedSubMenuItem.Background = DarkModeSecondaryBackground;
                                    extendedSubMenuItem.Foreground = DarkModeText;
                                    extendedSubMenuItem.BorderBrush = DarkModeSecondaryBackground;
                                }
                                subItem.Background = DarkModeSecondaryBackground;
                                subItem.Foreground = DarkModeText;
                                subItem.BorderBrush = DarkModeSecondaryBackground;
                            }
                            item.Background = DarkModeSecondaryBackground;
                            item.Foreground = DarkModeText;
                            item.BorderBrush = DarkModeSecondaryBackground;
                            
                        }
                    }else if (control.GetType() == typeof(TextBlock))
                    {
                        control.Foreground = DarkModeText;
                    }
                }
        }
    

    public static void ChangeGlobalFontColor(Panel Content)
        {
            if (DarkThemeEnabled)
            {
                return;
            }
            // casting the content into panel
            var mainContainer = Content;

            // GetAll UIElement
            var element = mainContainer.Children;

            // casting the UIElementCollection into List
            var lstElement = element.Cast<FrameworkElement>().ToList();

            // Getting all Control from list
            var lstControl = lstElement.OfType<Control>();

            foreach (var control in lstControl)

                // If the control is not the minimize or exit function.
                if (!control.Name.Contains("Program") && !control.Name.Contains("Strength"))
                {
                    control.Foreground = new SolidColorBrush(App.FontColor);
                }
        }

        
    }
}
