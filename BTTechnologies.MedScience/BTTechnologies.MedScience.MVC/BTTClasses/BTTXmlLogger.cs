using System;
using System.IO;
using System.Xml;

namespace BTTechnologies.MedScience.MVC.BTTClasses
{
    public class BTTXmlLogger
    {
        public const int MaxEntitiesCount = 500;
        public const string FileMainNode = "ExceptionsLog";
        public const string ExceptionNodeName = "Exception";
        public const string TimeAttrName = "DateTime";
        public const string MessageAttrName = "Message";
        public const string SourceAttrName = "Source";

        private readonly static object LockObject = new object();

        public string FullFileName { get; private set; }

        public BTTXmlLogger(string fullFuleName)
        {
            FullFileName = fullFuleName;
        }

        public void LogExceptionToFile(Exception exception)
        {
            if (string.IsNullOrEmpty(FullFileName) || exception == null)
                return;

            CreateNewLogFileIfNeeded(FullFileName);
            WriteExceptionToFile(FullFileName, exception);
        }

        private static void WriteExceptionToFile(string path, Exception exception)
        {
            lock (LockObject)
            {
                XmlDocument document = new XmlDocument();
                document.Load(path);
                XmlNode mainNode = document.SelectSingleNode(FileMainNode);
                
                XmlElement exeptionNode = document.CreateElement(ExceptionNodeName);
                exeptionNode.SetAttribute(TimeAttrName, DateTime.Now.ToString());
                exeptionNode.SetAttribute(MessageAttrName, exception.Message);
                exeptionNode.SetAttribute(SourceAttrName, exception.Source);
                exeptionNode.InnerText = exception.StackTrace;

                mainNode.AppendChild(exeptionNode);

                while (MaxEntitiesCount < mainNode.ChildNodes.Count)
                {
                    mainNode.RemoveChild(mainNode.ChildNodes[0]);
                }

                document.Save(path);    
            }
        }

        private static void CreateNewLogFileIfNeeded(string path)
        {
            lock (LockObject)
            {
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                    Directory.CreateDirectory(Path.GetDirectoryName(path));

                if (File.Exists(path))
                    return;

                using (XmlWriter writer = XmlWriter.Create(path, new XmlWriterSettings()))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement(FileMainNode);
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Flush();
                    writer.Close();
                }
            }
        }
    }
}