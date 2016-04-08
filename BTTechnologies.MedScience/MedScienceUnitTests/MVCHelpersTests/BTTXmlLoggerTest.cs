using System;
using System.IO;
using System.Xml;
using BTTechnologies.MedScience.MVC.BTTClasses;
using MedScienceUnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MedScienceUnitTests.MVCHelpersTests
{
    /// <summary>
    /// Summary description for BTTXmlLoggerTest
    /// </summary>
    [TestClass]
    public class BTTXmlLoggerTest
    {
        [TestMethod]
        public void AddLogItemTest()
        {
            using (TempDirectoryProvider logDirectoryProvider = new TempDirectoryProvider())
            {
                DateTime timeBefore = DateTime.Now.AddMinutes(-1);

                string fileName = Path.Combine(logDirectoryProvider.TempDirectoryPath, "FileDirectory", "Log.xml");
                BTTXmlLogger logger = new BTTXmlLogger(fileName);

                Exception exception = new AggregateException {Source = "Test is source"};
                logger.LogExceptionToFile(exception);

                Assert.IsTrue(File.Exists(fileName));
                XmlElement element = GetLastNode(fileName, 1);

                Assert.AreEqual(element.GetAttribute(BTTXmlLogger.MessageAttrName), exception.Message);
                Assert.AreEqual(element.GetAttribute(BTTXmlLogger.SourceAttrName), exception.Source);

                string time = element.GetAttribute(BTTXmlLogger.TimeAttrName);
                DateTime dateTime = DateTime.Parse(time);

                Assert.IsTrue(timeBefore <= dateTime && dateTime <= DateTime.Now);

                Exception exception2 = new AccessViolationException {Source = "Test is source 2"};
                logger.LogExceptionToFile(exception2);

                Assert.IsTrue(File.Exists(fileName));
                XmlElement element2 = GetLastNode(fileName, 2);

                Assert.AreEqual(element2.GetAttribute(BTTXmlLogger.MessageAttrName), exception2.Message);
                Assert.AreEqual(element2.GetAttribute(BTTXmlLogger.SourceAttrName), exception2.Source);

                time = element.GetAttribute(BTTXmlLogger.TimeAttrName);
                dateTime = DateTime.Parse(time);

                Assert.IsTrue(timeBefore <= dateTime && dateTime <= DateTime.Now);
            }
        }

        [TestMethod]
        public void MaxLogItemsTest()
        {
            using (TempDirectoryProvider logDirectoryProvider = new TempDirectoryProvider())
            {
                string fileName = Path.Combine(logDirectoryProvider.TempDirectoryPath, "FileDirectory", "Log.xml");
                BTTXmlLogger logger = new BTTXmlLogger(fileName);

                for (int i = 0; i < BTTXmlLogger.MaxEntitiesCount; i++)
                {
                    logger.LogExceptionToFile(new Exception { Source = i.ToString() });
                }

                GetLastNode(fileName, BTTXmlLogger.MaxEntitiesCount);

                logger.LogExceptionToFile(new Exception { Source = BTTXmlLogger.MaxEntitiesCount.ToString() });

                XmlElement element = GetLastNode(fileName, BTTXmlLogger.MaxEntitiesCount);
                element.GetAttribute(BTTXmlLogger.SourceAttrName, BTTXmlLogger.MaxEntitiesCount.ToString());

                element = GetFirstNode(fileName, BTTXmlLogger.MaxEntitiesCount);
                element.GetAttribute(BTTXmlLogger.SourceAttrName, 1.ToString());
            }
        }

        private XmlElement GetLastNode(string path, int countToVerify)
        {
            XmlDocument document = new XmlDocument();
            document.Load(path);
            XmlNode listNode = document.SelectSingleNode(BTTXmlLogger.FileMainNode);

            Assert.AreEqual(listNode.ChildNodes.Count, countToVerify);

            XmlElement element = listNode.ChildNodes[listNode.ChildNodes.Count - 1] as XmlElement;
            return element;
        }

        private XmlElement GetFirstNode(string path, int countToVerify)
        {
            XmlDocument document = new XmlDocument();
            document.Load(path);
            XmlNode listNode = document.SelectSingleNode(BTTXmlLogger.FileMainNode);

            Assert.AreEqual(listNode.ChildNodes.Count, countToVerify);

            XmlElement element = listNode.ChildNodes[0] as XmlElement;
            return element;
        }
    }
}
