
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PrivateGuard.Database_Tools
{
    /// <summary>
    /// Interaction logic for Contact.xaml
    /// </summary>
    public partial class Contact
    {
        public Contact()
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

        private void ReportButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(MessageBox.Show(
                "To contact me please email jprivateguard@gmail.com.\nIf you are reporting an issue please select \"Yes\" on this message prompt.\nSelecting \"Yes\" will copy the message in the correct format to your clipboard.\nAfter it is coped send me the email by pasting it into your email client.\nThe subject should be \"Private Guard Issue\".",
                "Report Issue.", MessageBoxButton.YesNo, MessageBoxImage.Information) != MessageBoxResult.Yes)
            {
                return;
            }
            var issue = new StringBuilder();
            issue.AppendLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
            issue.AppendLine("Issue Submitted At: " + DateTime.Now);
            issue.AppendLine("Windows OS Version: " + Environment.OSVersion);
            issue.AppendLine("Is 64-Bit: " + Environment.Is64BitOperatingSystem);
            issue.AppendLine("Processor Count: " + Environment.ProcessorCount);
            issue.AppendLine("Private Guard Version: " + MainWindow.VersionID);
            issue.AppendLine("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=");
            issue.AppendLine("Critical Issue: " + CriticalIssue.IsChecked);
            issue.AppendLine();
            issue.AppendLine(MainIssueTextBox.Text);
            Clipboard.SetText(issue.ToString());
            Close();
        }
    }
}
