using System.Collections.Generic;
using BTTechnologies.MedScience.Domain.Context;
using BTTechnologies.MedScience.Domain.Entities;

namespace BTTechnologies.MedScience.Domain.Abstract
{
    /// <summary>
    /// Documents repository
    /// </summary>
    public interface IDocumentsRepository
    {
        /// <summary>
        /// Table context
        /// </summary>
        MedScienceDBContext Context { get; }

        /// <summary>
        /// Get article by id
        /// </summary>
        Article GetArticleById(int id);

        /// <summary>
        /// Get article by display name
        /// </summary>
        Article GetArtilceByName(string displayName);

        /// <summary>
        /// Get category by id
        /// </summary>
        ArticleCategory GetCategoryById(int id);
        
        /// <summary>
        /// Get categories which name contains namePart
        /// </summary>
        IList<ArticleCategory> GetCategoryByNamePart(string namePart);

        /// <summary>
        /// Get category by display name
        /// </summary>
        ArticleCategory GetCategoryByDisplayName(string displayName);

        /// <summary>
        /// Add or update category
        /// </summary>
        void AddOrUpdateCategory(ArticleCategory category);

        /// <summary>
        /// Add or update article
        /// </summary>
        void AddUpdateArticle(Article article, IList<DocAttachment> oldAttachments, IList<ArticleCategory> oldCategories, string userLogin);

        /// <summary>
        /// Get author by id
        /// </summary>
        Author GetAuthorById(int id);

        /// <summary>
        /// Delete article
        /// </summary>
        /// <param name="article"></param>
        void DeleteArticle(Article article);

        /// <summary>
        /// Save uploaded files data to SiteFiles table
        /// </summary>
        /// <param name="files"></param>
        void AddUploadedFiles(IEnumerable<SiteFile> files);

        /// <summary>
        /// Get SiteFile by its id
        /// </summary>
        SiteFile GetSiteFileById(int id);

        /// <summary>
        /// Delete SiteFile
        /// </summary>
        void DeleteSiteFile(SiteFile siteFile);
    }
}
