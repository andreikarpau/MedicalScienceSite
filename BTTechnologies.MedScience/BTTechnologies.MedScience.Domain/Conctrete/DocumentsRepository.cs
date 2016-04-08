using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BTTechnologies.MedScience.Domain.Abstract;
using BTTechnologies.MedScience.Domain.Entities;
using BTTechnologies.MedScience.Domain.Helpers;

namespace BTTechnologies.MedScience.Domain.Conctrete
{
    /// <summary>
    /// Documents repository
    /// </summary>
    public class DocumentsRepository : BaseRepository, IDocumentsRepository
    {
        /// <summary>
        /// <see cref="IDocumentsRepository.GetCategoryByNamePart"/>
        /// </summary>
        public IList<ArticleCategory> GetCategoryByNamePart(string namePart)
        {
            return Context.ArticleCategories.Where(c => c.DisplayName.Contains(namePart)).ToList();
        }

        /// <summary>
        /// <see cref="IDocumentsRepository.GetCategoryByDisplayName"/>
        /// </summary>
        public ArticleCategory GetCategoryByDisplayName(string displayName)
        {
            return Context.ArticleCategories.FirstOrDefault(c => c.DisplayName == displayName);
        }

        /// <summary>
        /// <see cref="IDocumentsRepository.AddOrUpdateCategory"/>
        /// </summary>
        public void AddOrUpdateCategory(ArticleCategory category)
        {
            if (category.Id == 0)
            {
                Context.ArticleCategories.Add(category);
            }
            else
            {
                Context.Entry(category).State = EntityState.Modified;
            }

            Context.SaveChanges();
        }

        /// <summary>
        /// <see cref="IDocumentsRepository.AddUpdateArticle"/>
        /// </summary>
        public void AddUpdateArticle(Article article, IList<DocAttachment> oldAttachments, IList<ArticleCategory> oldCategories, string userLogin)
        {
            string info;

            article.LastChangedDate = DateTime.Now;

            if (article.Id <= 0)
            {
                article.CreatedDate = DateTime.Now;

                Context.Articles.Add(article);
                info = "Article added";
            }
            else
            {
                Context.Entry(article).State = EntityState.Modified;
                info = "Article updated";
            }

            foreach (DocAttachment docAttachment in oldAttachments)
                Context.DocAttachments.Remove(docAttachment);

            DeleteUnusedCategoriesFromList(oldCategories);
            Context.SaveChanges();

            AddArticleChangedLog(article, userLogin, info);
        }

        /// <summary>
        /// <see cref="IDocumentsRepository.DeleteArticle"/>
        /// </summary>
        public void DeleteArticle(Article article)
        {
            IList<DocAttachment> docAttachments = article.Attachments.ToList();
            foreach (DocAttachment docAttachment in docAttachments)
            {
                Context.DocAttachments.Remove(docAttachment);
            }

            DeleteUnusedCategoriesFromList(article.Categories);

            article.Attachments.Clear();
            article.Authors.Clear();
            article.Categories.Clear();
            Context.Articles.Remove(article);

            Context.SaveChanges();
        }

        /// <summary>
        /// <see cref="IDocumentsRepository.AddUploadedFiles"/>
        /// </summary>
        /// <param name="files"></param>
        public void AddUploadedFiles(IEnumerable<SiteFile> files)
        {
            if (files == null || !files.Any())
                return;

            foreach (SiteFile file in files)
            {
                Context.SiteFiles.Add(file);
            }

            Context.SaveChanges();
        }

        /// <summary>
        /// <see cref="IDocumentsRepository.GetSiteFileById"/>
        /// </summary>
        public SiteFile GetSiteFileById(int id)
        {
            return Context.SiteFiles.FirstOrDefault(s => s.Id == id);
        }

        /// <summary>
        /// <see cref="IDocumentsRepository.DeleteSiteFile"/>
        /// </summary>
        public void DeleteSiteFile(SiteFile siteFile)
        {
            Context.SiteFiles.Remove(siteFile);
            Context.SaveChanges();
        }

        private void DeleteUnusedCategoriesFromList(IEnumerable<ArticleCategory> categories)
        {
            foreach (ArticleCategory articleCategory in categories.Where(articleCategory => !articleCategory.Articles.Any()))
            {
                Context.ArticleCategories.Remove(articleCategory);
            }
        }

        private void AddArticleChangedLog(Article article, string userLogin, string info)
        {
            try
            {
                Account account = Context.Accounts.FirstOrDefault(a => a.UserLogin == userLogin);

                ArticleChangesLog logEntry = new ArticleChangesLog
                {
                    AccountId = account != null ? account.Id : 0,
                    ChangesInformation = info,
                    ItemId = article.Id,
                    ItemName = article.DisplayName,
                    LoginWhoChanged = userLogin
                };

                Context.ArticleChangesLogs.Add(logEntry);
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                ExceptionsLogger.LogException(ex);
            }
        }
    }
}
