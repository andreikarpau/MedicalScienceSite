using System.Collections.Generic;
using System.Linq;
using BTTechnologies.MedScience.Domain.Entities;

namespace BTTechnologies.MedScience.Domain.Abstract
{
    /// <summary>
    /// Articles Repository
    /// </summary>
    public interface IArticlesRepository
    {
        /// <summary>
        /// Get ordered articles which display names begin with string
        /// </summary>
        /// <returns></returns>
        IOrderedQueryable<Article> GetArticlesByNamePatternOrderByName(string beginString);
        
        /// <summary>
        /// Get articles ordered by modification date which display names begin with string
        /// </summary>
        /// <returns></returns>
        IOrderedQueryable<Article> GetArticlesByNamePatternOrderByModificationDate(string beginString);    
    
        /// <summary>
        /// Get articles ordered by created date which display names begin with string
        /// </summary>
        /// <returns></returns>
        IOrderedQueryable<Article> GetArticlesByNamePatternOrderByCreatedDate(string beginString);

        /// <summary>
        /// Get all published articles
        /// </summary>
        /// <returns></returns>
        IEnumerable<Article> GetAllPublishedArticles();

        /// <summary>
        /// Get article by id
        /// </summary>
        Article GetPublishedArticleById(int id);

        /// <summary>
        /// Get authors who have articles
        /// </summary>
        IList<Author> GetAuthorsWithArticles();

        /// <summary>
        /// Get categories who have articles
        /// </summary>
        IList<ArticleCategory> GetCategoriesWithArticles();

        /// <summary>
        /// Get category by id
        /// </summary>
        ArticleCategory GetCategoryById(int id);

        /// <summary>
        /// Get author by id
        /// </summary>
        Author GetAuthorById(int id);

        /// <summary>
        /// Get articles by name pattern
        /// </summary>
        IQueryable<Article> SearchArticleByNamePattern(string pattern);

        /// <summary>
        /// Get articles by description pattern, ignore case
        /// </summary>
        IList<Article> SearchArticleByDescriptionPattern(string pattern);

        /// <summary>
        /// Get categories by category name pattern, ignore case
        /// </summary>
        IList<ArticleCategory> SearchArticleByCategoryNamePattern(string pattern);

        /// <summary>
        /// Get authors by author name pattern, ignore case
        /// </summary>
        IList<Author> SearchArticleByAuthorNamePattern(string pattern);
    }
}
