using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PrivateGuard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    
    public partial class App : Application
    {
        public static string Font = "Trebuchet MS";
        public static Color FontColor = Color.FromRgb(0,0,0);
        public static Color DatabaseColor = Color.FromRgb(240, 240, 240);

        public static void ChangeGlobalFontColor(Panel Content)
        {
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
