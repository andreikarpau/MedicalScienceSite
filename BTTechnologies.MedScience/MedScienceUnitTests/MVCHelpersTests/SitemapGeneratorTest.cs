using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using BTTechnologies.MedScience.MVC.Helpers.SitemapGenerator;
using MedScienceUnitTests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MedScienceUnitTests.MVCHelpersTests
{
    [TestClass]
    public class SitemapGeneratorTest
    {
        private readonly IEnumerable<ISitemapItem> items1 = new List<SitemapItem>
        {
            new SitemapItem("http://medicinescience.ru/ReadArticles/ShowArticleView/1", new DateTime(2014, 1, 1)),
            new SitemapItem("http://medicinescience.ru/ReadArticles/ShowArticleView/2", changeFrequency: SitemapChangeFrequency.Daily),
            new SitemapItem("http://medicinescience.ru/ReadArticles/ShowArticleView/3", priority: 1),
            new SitemapItem("http://medicinescience.ru/ReadArticles/ShowArticleView/4"),
            new SitemapItem("http://medicinescience.ru/ReadArticles/ShowArticleView/5"),
            new SitemapItem("http://medicinescience.ru/"),
            new SitemapItem("http://medicinescience.ru/ReadArticles/ShowAllArticlesList"),
        };

        private readonly IEnumerable<ISitemapItem> items2 = new List<SitemapItem>
        {
            new SitemapItem("http://medicinescience.ru/ReadArticles/ShowArticleView/15", DateTime.Now.Date, SitemapChangeFrequency.Hourly, 0.9),
            new SitemapItem("http://medicinescience.ru/ReadArticles/ShowArticleView/16", changeFrequency: SitemapChangeFrequency.Always),
            new SitemapItem("http://medicinescience.ru/ReadArticles/ShowArticleView/17", changeFrequency:SitemapChangeFrequency.Weekly, priority: 0.1),
        };



        [TestMethod]
        public void GenerateSitemapTest()
        {
            DateTime startDateTime = DateTime.Now.AddDays(-1);

            using (TempDirectoryProvider directoryProvider = new TempDirectoryProvider())
            {
                SitemapGeneratorForTest generator = new SitemapGeneratorForTest(directoryProvider.TempDirectoryPath);
                // Create sitemap
                generator.WriteSitemapToFile(items1, true);
                CheckSitemapFiles(generator, items1, startDateTime);

                // Rewrite sitemap
                generator.WriteSitemapToFile(items2, true);
                CheckSitemapFiles(generator, items2, startDateTime);
            }
        }

        private void CheckSitemapFiles(SitemapGeneratorForTest generator, IEnumerable<ISitemapItem> items, DateTime startTime)
        {
            int sitemapFilesCount = (int)Math.Ceiling((double)items.Count() / generator.MaxRecordsCount);
            Assert.AreEqual(Directory.GetFiles(generator.PublicBasePath).Count(), sitemapFilesCount + 1);
            
            string siteMapIndexFile = generator.SitemapIndexPath;
            Assert.AreEqual(siteMapIndexFile.Trim(), Path.Combine(generator.PublicBasePath, generator.SitemapIndexFile).Trim());
            Assert.IsTrue(File.Exists(siteMapIndexFile));

            IList<string> sitemapFilesNames = new List<string>();

            using (FileStream stream = new FileStream(siteMapIndexFile, FileMode.Open))
            {
                XDocument document = XDocument.Load(stream);
                Assert.IsTrue(string.Equals(document.Declaration.ToString(), "<?xml version=\"1.0\" encoding=\"UTF-8\"?>", StringComparison.InvariantCultureIgnoreCase));

                Assert.AreEqual(document.Elements().Count(), 1);
                XElement sitemapIndexElement = document.Elements().First();
                Assert.AreEqual(sitemapIndexElement.Name.LocalName, "sitemapindex");

                Assert.AreEqual(sitemapIndexElement.Attribute("xmlns").Value, "http://www.sitemaps.org/schemas/sitemap/0.9");

                XAttribute locationAttribute = sitemapIndexElement.Attributes().First(a => a.Name.LocalName.Equals("schemaLocation"));
                Assert.AreEqual(locationAttribute.Value, "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");

                XAttribute xsiAttribute = sitemapIndexElement.Attributes().First(a => a.Name.LocalName.Equals("xsi"));
                Assert.AreEqual(xsiAttribute.Value, "http://www.w3.org/2001/XMLSchema-instance");

                Assert.AreEqual(sitemapIndexElement.Elements().Count(), sitemapFilesCount);

                foreach (XElement element in sitemapIndexElement.Elements())
                {
                    Assert.AreEqual(element.Name.LocalName, "sitemap");
                    Assert.AreEqual(element.Elements().Count(), 2);

                    XElement loc = element.Elements().First();
                    Assert.AreEqual(loc.Name.LocalName, "loc");
                    sitemapFilesNames.Add(loc.Value);

                    XElement lastmod = element.Elements().Last();
                    Assert.AreEqual(lastmod.Name.LocalName, "lastmod");
                    DateTime dateTime = DateTime.ParseExact(lastmod.Value, SitemapGeneratorForTest.DateFormat, null);
                    Assert.IsTrue((startTime <= dateTime) && (dateTime <= DateTime.Now));
                }

                int index = 0;
                foreach (string fileName in sitemapFilesNames)
                {
                    CheckSitemapFile(fileName, items.Skip(index * generator.MaxRecordsCount).Take(generator.MaxRecordsCount), generator.PublicBasePath);
                    index++;
                }
            }
        }

        private void CheckSitemapFile(string fileUrl, IEnumerable<ISitemapItem> items, string basePath)
        {
            string fileName = fileUrl.Replace(@"~/", string.Empty);
            string filePath = Path.Combine(basePath, fileName);

            Assert.IsTrue(File.Exists(filePath));
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                XDocument document = XDocument.Load(stream);
                Assert.IsTrue(string.Equals(document.Declaration.ToString(), "<?xml version=\"1.0\" encoding=\"UTF-8\"?>", StringComparison.InvariantCultureIgnoreCase));
                Assert.AreEqual(document.Elements().Count(), 1);
                XElement urlsetElement = document.Elements().First();
                Assert.AreEqual(urlsetElement.Name.LocalName, "urlset");
                Assert.AreEqual(urlsetElement.Elements().Count(), items.Count());

                Assert.AreEqual(urlsetElement.Attribute("xmlns").Value, "http://www.sitemaps.org/schemas/sitemap/0.9");

                XAttribute locationAttribute = urlsetElement.Attributes().First(a => a.Name.LocalName.Equals("schemaLocation"));
                Assert.AreEqual(locationAttribute.Value, "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");

                XAttribute xsiAttribute = urlsetElement.Attributes().First(a => a.Name.LocalName.Equals("xsi"));
                Assert.AreEqual(xsiAttribute.Value, "http://www.w3.org/2001/XMLSchema-instance");


                foreach (XElement urlElement in urlsetElement.Elements())
                {
                    Assert.AreEqual(urlElement.Name.LocalName, "url");
                    XElement locElement = urlElement.Elements().First();
                    ISitemapItem sitemapItem = items.First(i => string.Equals(i.Url, locElement.Value, StringComparison.InvariantCultureIgnoreCase));

                    int elementsCount = 1;
                    if (sitemapItem.LastModified.HasValue)
                    {
                        XElement lastModifiedElement = urlElement.Elements().First(e => string.Equals(e.Name.LocalName, "lastmod", StringComparison.InvariantCultureIgnoreCase));
                        DateTime dateTime = DateTime.ParseExact(lastModifiedElement.Value, SitemapGeneratorForTest.DateFormat, null);
                        Assert.IsTrue(sitemapItem.LastModified.Equals(dateTime));
                        elementsCount++;
                    }

                    if (sitemapItem.ChangeFrequency.HasValue)
                    {
                        XElement frequencyElement = urlElement.Elements().First(e => string.Equals(e.Name.LocalName, "changefreq", StringComparison.InvariantCultureIgnoreCase));
                        Assert.IsTrue(string.Equals(frequencyElement.Value, sitemapItem.ChangeFrequency.Value.ToString(), StringComparison.InvariantCultureIgnoreCase));
                        elementsCount++;
                    }

                    if (sitemapItem.Priority.HasValue)
                    {
                        XElement priorityElement = urlElement.Elements().First(e => string.Equals(e.Name.LocalName, "priority", StringComparison.InvariantCultureIgnoreCase));
                        double priority = double.Parse(priorityElement.Value, CultureInfo.InvariantCulture);
                        Assert.AreEqual(priority, sitemapItem.Priority);
                        elementsCount++;
                    }

                    Assert.AreEqual(elementsCount, urlElement.Elements().Count());
                }
            }
        }
    }

    public class SitemapGeneratorForTest : SitemapGenerator
    {
        private readonly string path;

        public static string DateFormat {get { return SiteMapDateFormat; }}

        public string SitemapIndexFile
        {
            get { return SitemapIndexFileName; }
        }

        protected override string BasePath
        {
            get { return path; }
        }

        public string PublicBasePath
        {
            get { return BasePath; }
        }

        protected override int MaxSitemapRecordsCount
        {
            get
            {
                return 5;
            }
        }

        public int MaxRecordsCount
        {
            get { return MaxSitemapRecordsCount; }
        }

        public SitemapGeneratorForTest(string basePath)
        {
            path = basePath;
        }
    }
}
