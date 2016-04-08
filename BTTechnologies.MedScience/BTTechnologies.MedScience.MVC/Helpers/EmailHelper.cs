using System.Net;
using System.Net.Mail;

namespace BTTechnologies.MedScience.MVC.Helpers
{
    public static class EmailHelper
    {
        public static void SendActivationCodeEmail(string address, string code, string activationUrl)
        {
            SendEmail(address, ConfigurationManagerHelper.ActivationEmailSubject, string.Format(ConfigurationManagerHelper.ActivationEmailText, code, activationUrl));
        }
        
        public static void SendRestoreCodeEmail(string address, string code)
        {
            SendEmail(address, ConfigurationManagerHelper.RestoreEmailSubject, string.Format(ConfigurationManagerHelper.RestoreEmailText, code));
        }

        public static void SendEmail(string address, string subject, string message)
        {
            if (string.IsNullOrEmpty(address))
                return;

            string email = ConfigurationManagerHelper.SiteEmail;
            string password = ConfigurationManagerHelper.SiteEmailPassword;
            string host = ConfigurationManagerHelper.SiteEmailHost;
            int port = ConfigurationManagerHelper.SiteEmailPort;
            
            var loginInfo = new NetworkCredential(email, password);
            var msg = new MailMessage();
            var smtpClient = new SmtpClient(host, port);

            msg.From = new MailAddress(email);
            msg.To.Add(new MailAddress(address));
            msg.Subject = subject ?? string.Empty;
            msg.Body = message ?? string.Empty;
            msg.IsBodyHtml = true;

            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = loginInfo;
            smtpClient.Send(msg);
        }
    }
}