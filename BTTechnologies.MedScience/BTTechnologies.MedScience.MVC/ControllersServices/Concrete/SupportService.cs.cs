using System;
using System.Collections.Generic;
using BTTechnologies.MedScience.Domain.Abstract;
using BTTechnologies.MedScience.Domain.Entities;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using BTTechnologies.MedScience.MVC.Helpers.SitemapGenerator;

namespace BTTechnologies.MedScience.MVC.ControllersServices.Concrete
{
    public class SupportService : ISupportService
    {
        private readonly IArticlesRepository articlesRepository;

        public SupportService(IArticlesRepository newArticlesRepository)
        {
            articlesRepository = newArticlesRepository;
        }

        public IList<ISitemapItem> GetPublishedArticlesSiteMaps(Func<int, string> getArticleUrl, Func<string, string> getAttachmentsUrl)
        {
            IList<ISitemapItem> sitemapItems = new List<ISitemapItem>();

            foreach (Article article in articlesRepository.GetAllPublishedArticles())
            {
                sitemapItems.Add(new SitemapItem(getArticleUrl(article.Id), article.LastChangedDate, priority:0.9));

                foreach (DocAttachment attachment in article.Attachments)
                {
                    sitemapItems.Add(new SitemapItem(getAttachmentsUrl(attachment.AttachmentURL), article.LastChangedDate, priority: 0.9));
                }
            }
            
            return sitemapItems;
        }
    }
}