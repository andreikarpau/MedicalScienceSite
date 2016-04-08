using System.Configuration;

namespace BTTechnologies.MedScience.MVC.Helpers
{
    public static class ConfigurationManagerHelper
    {
        public static string UploadArticleDocumentsPath { get { return ConfigurationManager.AppSettings["UploadDocumentsPath"]; } }
        public static string ActivationEmailSubject { get { return ConfigurationManager.AppSettings["ActivationEmailSubject"]; } }
        public static string RestoreEmailSubject { get { return ConfigurationManager.AppSettings["RestoreEmailSubject"]; } }
        public static string ActivationEmailText { get { return ConfigurationManager.AppSettings["ActivationEmailText"]; } }
        public static string RestoreEmailText { get { return ConfigurationManager.AppSettings["RestoreEmailText"]; } }
        public static string SiteEmail { get { return ConfigurationManager.AppSettings["SiteEmail"]; } }
        public static string SiteEmailPassword { get { return ConfigurationManager.AppSettings["SiteEmailPassword"]; } }
        public static string SiteEmailHost { get { return ConfigurationManager.AppSettings["SiteEmailHost"]; } }
        public static string LogFilePath { get { return ConfigurationManager.AppSettings["LogFilePath"]; } }
        public static string SiteUrl { get { return ConfigurationManager.AppSettings["SiteUrl"]; } }
        public static string UploadFilesPath { get { return ConfigurationManager.AppSettings["UploadFilesPath"]; } }

        public static int SiteEmailPort
        {
            get
            {
                int port = 0;
                string portStr = ConfigurationManager.AppSettings["SiteEmailPort"];
                if (string.IsNullOrEmpty(portStr))
                    return port;

                int.TryParse(portStr.Trim(), out port);
                return port;
            }
        }
    }
}