using System;
using System.Diagnostics;
using System.IO;
using BTTechnologies.MedScience.MVC.BTTClasses;

namespace BTTechnologies.MedScience.MVC.Helpers
{
    public static class ExceptionsLogger
    {
        private static readonly BTTXmlLogger Logger;

        static ExceptionsLogger()
        {
            try
            {
                Logger = new BTTXmlLogger(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManagerHelper.LogFilePath));
            }
            catch (Exception)
            {
                Debug.WriteLine("ExceptionsLogger: Cannot get LogFilePath");
            }
        }


        public static void LogException(Exception exception)
        {
            try
            {
                Logger.LogExceptionToFile(exception);
            }
            catch (Exception)
            {
                Debug.WriteLine("ExceptionsLogger: Error during writing exception to log occured");
                Debug.Assert(true);
            }
        }
    }
}