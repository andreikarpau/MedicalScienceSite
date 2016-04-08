using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Xml;
using BTTechnologies.MedScience.MVC.BTTClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MedScienceUnitTests.MVCHelpersTests
{
    [TestClass]
    public class BTTAjaxFileUploaderSaverTest
    {
        private string tempFolder;

        [TestInitialize]
        public void TestInitialize()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "BTTAjaxFileUploaderSaverTestFolder");
            if (Directory.Exists(tempPath))
                Directory.Delete(tempPath, true);
                
            Directory.CreateDirectory(tempPath);
            tempFolder = tempPath;
        }

        [TestMethod]
        public void TestEmptyAdd()
        {
            Tested saver = new Tested(tempFolder);
            UploadFilesOutputModel model = saver.SaveFilesToTempDirectory(new UploadFilesInputModel());
            Assert.IsFalse(model.UploadedFileInfos.Any());
        }

        [TestMethod]
        public void TestOneFileAddAndTimeDelete()
        {
            const string fileName = "tempFile1.doc";

            Tested saver = new Tested(tempFolder) { SecondsToDeleteValue = 1 };

            UploadFilesInputModel inputModel = new UploadFilesInputModel();
            inputModel.PostedFiles.Add(fileName, null);

            UploadFilesOutputModel model = saver.SaveFilesToTempDirectory(inputModel);
            Assert.AreEqual(model.UploadedFileInfos.Count, 1);
            Assert.IsTrue(saver.AddedFiles.First().Contains(model.UploadedFileInfos.First().FileGuid.ToString()));
            Assert.IsTrue(model.UploadedFileInfos.First().FileName.Equals(fileName));

            Thread.Sleep(saver.SecondsToDeleteValue * 2000);

            Assert.IsTrue(File.Exists(Path.Combine(tempFolder, model.UploadedFileInfos.First().FileGuid.ToString())));
            
            saver.SaveFilesToTempDirectory(new UploadFilesInputModel());
            
            Assert.IsFalse(File.Exists(Path.Combine(tempFolder, model.UploadedFileInfos.First().FileGuid.ToString())));
        }

        [TestMethod]
        public void TestManyFilesAddAndTimeDelete()
        {
            const string fileName1 = "tempTestFile1.doc";
            const string fileName2 = "tempTestFile2.doc";
            const string fileName3 = "tempTestFile3.doc";
            IList<string> fileNames = new List<string> {fileName1, fileName2, fileName3};

            Tested saver = new Tested(tempFolder) { SecondsToDeleteValue = 1};

            UploadFilesInputModel inputModel = new UploadFilesInputModel();
            inputModel.PostedFiles.Add(fileName1, null);
            inputModel.PostedFiles.Add(fileName2, null);
            inputModel.PostedFiles.Add(fileName3, null);

            UploadFilesOutputModel model = saver.SaveFilesToTempDirectory(inputModel);

            Assert.AreEqual(model.UploadedFileInfos.Count, 3);

            foreach (UploadFilesOutputModel.UploadedFileInfo info in model.UploadedFileInfos)
            {
                Assert.AreEqual(saver.AddedFiles.Count(s => s.Contains(info.FileGuid.ToString())), 1);
                Assert.AreEqual(fileNames.Count(n => n.Equals(info.FileName)), 1);
            }

            Thread.Sleep(saver.SecondsToDeleteValue * 2000);

            foreach (UploadFilesOutputModel.UploadedFileInfo info in model.UploadedFileInfos)
            {
                Assert.IsTrue(File.Exists(Path.Combine(tempFolder, info.FileGuid.ToString())));
            }

            saver.SaveFilesToTempDirectory(new UploadFilesInputModel());

            foreach (UploadFilesOutputModel.UploadedFileInfo info in model.UploadedFileInfos)
            {
                Assert.IsFalse(File.Exists(Path.Combine(tempFolder, info.FileGuid.ToString())));
            }

            saver.CheckXMLFileIsEmpty();
        }

        [TestMethod]
        public void GetFileNameMethodTest()
        {
            const string fileName1 = "tempTestFile1.doc";
            const string fileName2 = "tempTestFile2.doc";
            const string fileName3 = "tempTestFile3.doc";
            Tested saver = new Tested(tempFolder) { SecondsToDeleteValue = 1 };

            UploadFilesInputModel inputModel = new UploadFilesInputModel();
            inputModel.PostedFiles.Add(fileName1, null);
            inputModel.PostedFiles.Add(fileName2, null);
            inputModel.PostedFiles.Add(fileName3, null);

            UploadFilesOutputModel model = saver.SaveFilesToTempDirectory(inputModel);
            foreach (UploadFilesOutputModel.UploadedFileInfo info in model.UploadedFileInfos)
            {
                string fileName = saver.GetFileNameByGuid(info.FileGuid);
                Assert.AreEqual(fileName, info.FileName);
            }
        }

        [TestMethod]
        public void MoveFileToDirectoryTest()
        {
            const string fileName1 = "temp1File.doc";
            const string fileName1Changed = "temp1File1.doc";
            const string fileName2 = "temp2TestFile.doc";
            const string fileName3 = "temp3TestFile.doc";

            Tested saver = new Tested(tempFolder) { SecondsToDeleteValue = 1 };

            UploadFilesInputModel inputModel = new UploadFilesInputModel();
            inputModel.PostedFiles.Add(fileName1, null);
            inputModel.PostedFiles.Add(fileName2, null);
            inputModel.PostedFiles.Add(fileName3, null);

            UploadFilesOutputModel model = saver.SaveFilesToTempDirectory(inputModel);

            string copyToDirectoryName = Guid.NewGuid().ToString();
            copyToDirectoryName = Path.Combine(Path.GetTempPath(), copyToDirectoryName);
            if (!Directory.Exists(copyToDirectoryName))
                Directory.CreateDirectory(copyToDirectoryName);

            using (File.Create(Path.Combine(copyToDirectoryName, fileName1)))
            {
                
            }
            
            IList<string> resultFileNames = new List<string>();
            resultFileNames.Add(Path.Combine(copyToDirectoryName, fileName1Changed));
            resultFileNames.Add(Path.Combine(copyToDirectoryName, fileName2));
            resultFileNames.Add(Path.Combine(copyToDirectoryName, fileName3));

            foreach (UploadFilesOutputModel.UploadedFileInfo info in model.UploadedFileInfos)
            {
                string copiedFilePath = saver.MoveFileToDirectory(copyToDirectoryName, info.FileGuid);
                string path = resultFileNames.FirstOrDefault(p => p.Equals(copiedFilePath));
                Assert.IsNotNull(path);
                Assert.IsNotNull(File.Exists(path));

                resultFileNames.Remove(path);
            }

            Assert.IsTrue(File.Exists(Path.Combine(copyToDirectoryName, fileName1)));
            saver.CheckXMLFileIsEmpty();

            Directory.Delete(copyToDirectoryName, true);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Directory.Delete(tempFolder, true);
        }
    }

    public class Tested : BTTAjaxFileUploader
    {
        private readonly string folder;

        public int SecondsToDeleteValue { get; set; }
        public IList<string> AddedFiles { get; private set; }

        protected override int SecondsToDelete { get { return SecondsToDeleteValue; } }

        public Tested(string tempFolder)
            : base(tempFolder)
        {
            AddedFiles = new List<string>();
            SecondsToDeleteValue = 3600;
            CheckFileValue = false;
            folder = tempFolder;
        }

        public void CheckXMLFileIsEmpty()
        {
            XmlDocument document = new XmlDocument();
            document.Load(InfoFilePath);
            XmlNode listNode = document.SelectSingleNode(FilesListTag);
            Assert.IsFalse(listNode.HasChildNodes);
        }

        protected override void SaveFile(HttpPostedFileBase file, string filePath)
        {
            Assert.IsFalse(string.IsNullOrEmpty(filePath));

            string fileName = Path.GetFileName(filePath);
            Guid guid;

            Assert.IsTrue(Guid.TryParse(fileName, out guid));
            Assert.IsTrue(folder.Equals(Path.GetDirectoryName(filePath)));

            AddedFiles.Add(filePath);
            using (File.Create(filePath))
            {

            }
        }
    }
}
