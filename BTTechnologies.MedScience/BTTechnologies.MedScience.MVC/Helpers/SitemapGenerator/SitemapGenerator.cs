using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace BTTechnologies.MedScience.MVC.Helpers.SitemapGenerator
{
    /// <summary>
    /// A class for creating XML Sitemaps (see http://www.sitemaps.org/protocol.html)
    /// </summary>
    public class SitemapGenerator : ISitemapGenerator
    {
        private const int maxSitemapUrlsCount = 100;//25000;
        protected const string SiteMapDateFormat = "yyyy-MM-dd";
        protected const string SitemapFileTemplate = "Sitemap{0}.xml";
        protected const string SitemapIndexFileName = "SitemapIndex.xml";
        private static readonly XNamespace Xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        private static readonly XNamespace Xsi = "http://www.w3.org/2001/XMLSchema-instance";
        private static readonly XNamespace SchemaLocation = "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd";

        public Func<string, string> PathToFullUrlFunc { get; set; }  

        public virtual string SitemapIndexPath
        {
            get { return Path.Combine(BasePath, SitemapIndexFileName); }
        }

        protected virtual int MaxSitemapRecordsCount
        {
            get { return maxSitemapUrlsCount; }
        }

        protected virtual string BasePath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        public void WriteSitemapToFile(IEnumerable<ISitemapItem> items, bool rethrowExceptions = false)
        {
            try
            {
                if (items == null && !items.Any())
                    throw new ArgumentNullException();

                IEnumerable<IList<ISitemapItem>> filesContentsList = GetFilesContentLists(items);

                DeleteOldSitemapFiles(rethrowExceptions);
                IEnumerable<string> sitemapFiles = CreateSitemapFiles(filesContentsList, rethrowExceptions);
                RefreshSiteMapIndexFile(sitemapFiles);
            }
            catch (Exception e)
            {
                ExceptionsLogger.LogException(e);
                if (rethrowExceptions)
                    throw;
            }
        }
        
        public XDocument GenerateSiteMapDocument(IEnumerable<ISitemapItem> items)
        {
            if (items == null)
                throw new NullReferenceException();

            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                    new XElement(Xmlns + "urlset",
                      new XAttribute("xmlns", Xmlns),
                      new XAttribute(XNamespace.Xmlns + "xsi", Xsi),
                      new XAttribute(Xsi + "schemaLocation", SchemaLocation),
                      from item in items
                      select CreateItemElement(item)
                      )
                 );

            return sitemap;
        }

        public XDocument GenerateSiteMapIndexDocument(IEnumerable<string> fileNames)
        {
            if (fileNames == null)
                throw new NullReferenceException();

            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                    new XElement(Xmlns + "sitemapindex",
                      new XAttribute("xmlns", Xmlns),
                      new XAttribute(XNamespace.Xmlns + "xsi", Xsi),
                      new XAttribute(Xsi + "schemaLocation", SchemaLocation),
                      from file in fileNames
                      select CreateSitemapIndexElement(file)
                      )
                 );

            return sitemap;
        }
        
        protected virtual string GetFileName(int fileIndex)
        {
            return Path.Combine(BasePath, string.Format(SitemapFileTemplate, fileIndex));
        }

        private void RefreshSiteMapIndexFile(IEnumerable<string> sitemapFiles)
        {
            XDocument sitemapIndexDocument = GenerateSiteMapIndexDocument(sitemapFiles);

            using (FileStream stream = File.Create(SitemapIndexPath))
            {
                sitemapIndexDocument.Save(stream);
            }
        }

        private XElement CreateSitemapIndexElement(string fileName)
        {
            string url = PathToFullUrlFunc == null ? FilesHelper.GetFileUrlByPath(BasePath, fileName.ToLowerInvariant()) : PathToFullUrlFunc(FilesHelper.GetFileUrlByPath(BasePath, fileName.ToLowerInvariant()));

            var itemElement = new XElement(Xmlns + "sitemap", new XElement(Xmlns + "loc", url));
            itemElement.Add(new XElement(Xmlns + "lastmod", DateTime.Now.ToString(SiteMapDateFormat)));
            return itemElement;
        }

        private IEnumerable<string> CreateSitemapFiles(IEnumerable<IList<ISitemapItem>> filesContentsList, bool rethrowExceptions)
        {
            IList<string> newFileNames = new List<string>();

            int index = 1;
            foreach (IList<ISitemapItem> list in filesContentsList)
            {
                try
                {
                    string newFileName = GetFileName(index);
                    newFileNames.Add(newFileName);

                    using (FileStream stream = File.Create(newFileName))
                    {
                        XDocument document = GenerateSiteMapDocument(list);
                        document.Save(stream);
                    }
                }
                catch (Exception e)
                {
                    ExceptionsLogger.LogException(e);
                    if (rethrowExceptions)
                        throw;
                }

                index++;
            }

            return newFileNames;
        }

        private void DeleteOldSitemapFiles(bool rethrowExceptions)
        {
            int index = 1;

            while (File.Exists(GetFileName(index)))
            {
                try
                {
                    File.Delete(GetFileName(index));
                }
                catch (Exception e)
                {
                    ExceptionsLogger.LogException(e);
                    if (rethrowExceptions)
                        throw;
                }
                index++;
            }
        }

        private IEnumerable<IList<ISitemapItem>> GetFilesContentLists(IEnumerable<ISitemapItem> items)
        {
            IList<IList<ISitemapItem>> filesContentsList = new List<IList<ISitemapItem>>();
            IList<ISitemapItem> itemsList = items.ToList();
            while (MaxSitemapRecordsCount < itemsList.Count())
            {
                filesContentsList.Add(itemsList.Take(MaxSitemapRecordsCount).ToList());
                itemsList = itemsList.Skip(MaxSitemapRecordsCount).ToList();
            }

            if (itemsList.Any())
                filesContentsList.Add(itemsList);

            return filesContentsList;
        }

        private XElement CreateItemElement(ISitemapItem item)
        {
            var itemElement = new XElement(Xmlns + "url", new XElement(Xmlns + "loc", item.Url.ToLowerInvariant()));

            // all other elements are optional
            if (item.LastModified.HasValue)
                itemElement.Add(new XElement(Xmlns + "lastmod", item.LastModified.Value.ToString(SiteMapDateFormat)));

            if (item.ChangeFrequency.HasValue)
                itemElement.Add(new XElement(Xmlns + "changefreq", item.ChangeFrequency.Value.ToString().ToLower()));

            if (item.Priority.HasValue)
                itemElement.Add(new XElement(Xmlns + "priority", item.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));

            return itemElement;
        }
    }
}