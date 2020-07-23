using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PrivateGuard.Database_Tools
{
    /// <summary>
    ///     Interaction logic for SaveAsNewKey.xaml
    /// </summary>
    public partial class SaveAsNewKey
    {
        public SaveAsNewKey()
        {
            InitializeComponent();
            ChangeGlobalFont(App.Font);
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

        public string NewKey { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.IsFileKeyValid(FileKeyField.Text))
            {
                FileKeyField.Text = "";
                MessageBox.Show("File key not valid! Please meet the requirements listed.", "File Key Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                NewKey = FileKeyField.Text;
                Close();
            }
        }

        private void Generate_Secure_Key_Click(object sender, RoutedEventArgs e)
        {
            var rand = new Random();
            var gen = "";
            var passwordGenOptionals =
                "abcdefghikjlmnopqrstuvwxyz1234567890!@#$%^&*()[]{}:'>?/.<,ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            for (var i = 0; i < rand.Next(8, 32); i++)
                gen += passwordGenOptionals[rand.Next(0, passwordGenOptionals.Length - 1)];
            FileKeyField.Text = gen;
        }
    }
}