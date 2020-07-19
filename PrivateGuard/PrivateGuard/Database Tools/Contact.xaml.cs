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
    /// Interaction logic for Contact.xaml
    /// </summary>
    public partial class Contact : Window
    {
        public Contact()
        {
            InitializeComponent();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(NameField.Text) || string.IsNullOrWhiteSpace(ProblemTextBox.Text))
            {
                MessageBox.Show("Please make sure all required fields are filled out.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                client.Port = 587;
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;
                client.Timeout = 10000;
                client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("privateguardcustomer@gmail.com", "privateguard123");
                System.Net.Mail.MailMessage mm = new System.Net.Mail.MailMessage("privateguardcustomer@gmail.com", "jprivateguard@gmail.com", "Private Guard Customer Issue", $"Name:{NameField.Text} / Issue: {ProblemTextBox.Text}");
                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.DeliveryNotificationOptions = System.Net.Mail.DeliveryNotificationOptions.OnFailure;
                client.Send(mm);
                MessageBox.Show("Sent Message.\nReply will be sent within 1 week.\nPlease do not contact more than one time every day.", "Success.", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            } catch (Exception)
            {
                MessageBox.Show("Error. Could not contact directly.\nPlease email jprivateguard@gmail.com with your issue instead.", "Error Sending.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            

        }
    }
}
