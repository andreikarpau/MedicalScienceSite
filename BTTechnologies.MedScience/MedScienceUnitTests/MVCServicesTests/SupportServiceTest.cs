using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using BTTechnologies.MedScience.Domain.Abstract;
using BTTechnologies.MedScience.Domain.Entities;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using BTTechnologies.MedScience.MVC.ControllersServices.Concrete;
using BTTechnologies.MedScience.MVC.Helpers.SitemapGenerator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MedScienceUnitTests.MVCServicesTests
{
    [TestClass]
    public class SupportServiceTest
    {
        private const string articleUrlPrefix = "http://article/";
        private const string docUrlPrefix = "http://doc/";

        private static ISupportService service;

        private static readonly IList<Article> Articles = new List<Article>
        {
            new Article {DisplayName = "article 3", DocumentDescription = "Description " + "aaa", Id = 3},
            new Article
            {
                Id = 8,
                DisplayName = "article 8",
                DocumentDescription = "descr 1111",
                Attachments = new Collection<DocAttachment>
                {
                    new DocAttachment {DisplayName = "DocAttachment1", AttachmentURL = "/att1", Id = 11, AttachmentType = ".pdf"},
                    new DocAttachment {DisplayName = "DocAttachment3", AttachmentURL = "/att3", Id = 13},
                    new DocAttachment {DisplayName = "DocAttachment5", AttachmentURL = "/att5", Id = 15, AttachmentType = ".doc"}
                }
            }
        };



        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Mock<IArticlesRepository> articlesRepositoryMock = new Mock<IArticlesRepository>();
            articlesRepositoryMock.Setup(repository => repository.GetAllPublishedArticles()).Returns(() => Articles);
            service = new SupportService(articlesRepositoryMock.Object);
        }
        
        [TestMethod]
        public void TestGetPublishedArticlesSiteMaps()
        {
            IList<ISitemapItem> items = service.GetPublishedArticlesSiteMaps(GetArticleUrl, GetDocUrl);
            Assert.AreEqual(items.Count, Articles.Count + Articles[1].Attachments.Count);

            foreach (ISitemapItem item in items)
            {
                if (item.Url.Contains(articleUrlPrefix))
                {
                    Article article = Articles.First(a => a.Id.ToString().Equals(item.Url.Substring(articleUrlPrefix.Length)));
                    Assert.AreEqual(item.LastModified, article.LastChangedDate);
                }
                else
                {
                    DocAttachment attachment = Articles[1].Attachments.First(a => a.AttachmentURL.Equals(item.Url.Substring(docUrlPrefix.Length)));
                    Assert.AreEqual(item.LastModified, Articles[1].LastChangedDate);
                }

                Assert.AreEqual(item.Priority, 0.9);
            }
        }

        private string GetArticleUrl(int i)
        {
            return articleUrlPrefix + i;
        }

        private string GetDocUrl(string str)
        {
            return docUrlPrefix + str;
        }
    }
}
