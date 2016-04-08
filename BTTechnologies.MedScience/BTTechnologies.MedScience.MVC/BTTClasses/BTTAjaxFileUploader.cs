using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace BTTechnologies.MedScience.MVC.BTTClasses
{
    public class BTTAjaxFileUploader
    {
        private const string defaultUploadDirectory = "BTTAjaxFileUploaderTempFolder";
        private const string uploaderInformationXmlFile = "UploaderInfo.xml";
        protected const string FilesListTag = "FilesInfos";
        private const string filePathAttributeName = "filePath"; 
        private const string fileTimeAttributeName = "fileTime";
        private const string realFileNameAttribute = "realFileName";
        private const string guidElementName = "element{0}";
        private const int defaultTimeToDeleteInSeconds = 14400; // 4 hours
        
        private static readonly object LockObj = new object();
        protected bool CheckFileValue = true; // used for unit testing

        public string FileSaverPath { get; private set; }
        
        protected string InfoFilePath { get { return Path.Combine(FileSaverPath, uploaderInformationXmlFile); } }

        protected virtual int SecondsToDelete { get { return defaultTimeToDeleteInSeconds; } }

        public BTTAjaxFileUploader()
        {
            FileSaverPath = AppDomain.CurrentDomain.BaseDirectory + defaultUploadDirectory;
        }

        public BTTAjaxFileUploader(string path)
        {
            FileSaverPath = path;
        }

        public UploadFilesOutputModel SaveFilesToTempDirectory(UploadFilesInputModel model)
        {
            UploadFilesOutputModel outputModel = new UploadFilesOutputModel();
            DeleteOldFilesFromDirectory(InfoFilePath, SecondsToDelete);

            if (!model.PostedFiles.Any() || string.IsNullOrEmpty(FileSaverPath))
                return outputModel;

            if (!Directory.Exists(FileSaverPath))
                Directory.CreateDirectory(FileSaverPath);

            foreach (KeyValuePair<string, HttpPostedFileBase> file in model.PostedFiles)
            {
                if (string.IsNullOrEmpty(file.Key) || (CheckFileValue && file.Value == null))
                    continue;
                
                Guid fileGuid = Guid.NewGuid();
                string filePath = Path.Combine(FileSaverPath, fileGuid.ToString());

                SaveFile(file.Value, filePath);
                WriteNewFileInformation(file.Key, fileGuid, filePath, InfoFilePath);
                outputModel.UploadedFileInfos.Add(new UploadFilesOutputModel.UploadedFileInfo(fileGuid, file.Key));
            }

            return outputModel;
        }

        public string GetFileNameByGuid(Guid guid)
        {
            return GetFileNameByGuid(InfoFilePath, guid);
        }

        public string MoveFileToDirectory(string directoryToMove, Guid fileGuid)
        {
            string currentFilePath = Path.Combine(FileSaverPath, fileGuid.ToString());
            string realFileName = GetFileNameByGuid(fileGuid);

            if (string.IsNullOrEmpty(directoryToMove) || !File.Exists(currentFilePath) || string.IsNullOrEmpty(realFileName))
                return string.Empty;

            if (!Directory.Exists(directoryToMove))
                Directory.CreateDirectory(directoryToMove);

            int index = 1;
            string movedFileName = Path.GetFileNameWithoutExtension(realFileName);
            string movedFileExtension = Path.GetExtension(realFileName);
            while (File.Exists(Path.Combine(directoryToMove, movedFileName + movedFileExtension)))
            {
                movedFileName += index;
                index++;
            }

            string newFileName = Path.Combine(directoryToMove, movedFileName + movedFileExtension);
            File.Move(currentFilePath, newFileName);

            DeleteFileElementFromInfoFile(InfoFilePath, fileGuid);

            return newFileName;
        }

        private static string GetFileNameByGuid(string infoFilePath, Guid guid)
        {
            lock (LockObj)
            {
                if (!File.Exists(infoFilePath))
                    return string.Empty;

                try
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(infoFilePath);
                    XmlNode listNode = document.SelectSingleNode(FilesListTag);
                    XmlElement element = listNode.SelectSingleNode(GetGuidElementName(guid)) as XmlElement;
                    return element.GetAttribute(realFileNameAttribute);
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        protected virtual void SaveFile(HttpPostedFileBase file, string filePath)
        {
            file.SaveAs(filePath);
        }

        private static void DeleteFileElementFromInfoFile(string infoFilePath, Guid guid)
        {
            lock (LockObj)
            {
                if (!File.Exists(infoFilePath))
                    return;

                XmlDocument document = new XmlDocument();
                document.Load(infoFilePath);
                XmlNode listNode = document.SelectSingleNode(FilesListTag);
                XmlNode element = listNode.SelectSingleNode(GetGuidElementName(guid));

                if (element != null)
                    listNode.RemoveChild(element);

                document.Save(infoFilePath);
            }
        }

        private static void DeleteOldFilesFromDirectory(string infoFilePath, int secondsToDelete)
        {
            lock (LockObj)
            {
                if (!File.Exists(infoFilePath))
                    return;

                XmlDocument document = new XmlDocument();
                document.Load(infoFilePath);
                XmlNode listNode = document.SelectSingleNode(FilesListTag);

                DateTime now = DateTime.Now;

                IList<XmlElement> nodesToDelete = new List<XmlElement>();

                foreach (XmlElement node in listNode.ChildNodes)
                {
                    string nodeDate = node.GetAttribute(fileTimeAttributeName);
                    DateTime nodeDateTime;
                    if (string.IsNullOrEmpty(nodeDate) || !DateTime.TryParse(nodeDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out nodeDateTime))
                        continue;

                    TimeSpan difference = now - nodeDateTime;
                    if (secondsToDelete < difference.Seconds)
                        nodesToDelete.Add(node);
                }

                foreach (XmlElement xmlElement in nodesToDelete)
                {
                    DeleteFile(xmlElement.GetAttribute(filePathAttributeName));
                    listNode.RemoveChild(xmlElement);
                }

                document.Save(infoFilePath);
            }
        }

        protected static void DeleteFile(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        private static void WriteNewFileInformation(string realFileName, Guid fileGuid, string filePath, string infoFilePath)
        {
            lock (LockObj)
            {
                if (!File.Exists(infoFilePath))
                {
                    CreateNewInfoFile(infoFilePath);
                }

                XmlDocument document = new XmlDocument();
                document.Load(infoFilePath);
                XmlNode userNode = document.SelectSingleNode(FilesListTag);

                XmlElement fileNode = document.CreateElement(GetGuidElementName(fileGuid));
                fileNode.SetAttribute(filePathAttributeName, filePath);
                fileNode.SetAttribute(fileTimeAttributeName, DateTime.Now.ToString(CultureInfo.InvariantCulture));
                fileNode.SetAttribute(realFileNameAttribute, realFileName);

                userNode.AppendChild(fileNode);

                document.Save(infoFilePath);
            }
        }

        private static string GetGuidElementName(Guid fileGuid)
        {
            return string.Format(guidElementName, fileGuid);
        }

        private static void CreateNewInfoFile(string path)
        {
            if (File.Exists(path))
                return;

            using (XmlWriter writer = XmlWriter.Create(path, new XmlWriterSettings()))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement(FilesListTag);
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }
        }
    }
}