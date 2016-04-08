using System.Collections.Generic;
using System.Linq;
using BTTechnologies.MedScience.Domain.Abstract;
using BTTechnologies.MedScience.Domain.Entities;

namespace BTTechnologies.MedScience.Domain.Conctrete
{
    /// <summary>
    /// Articles repository
    /// </summary>
    public class ArticlesRepository : BaseRepository, IArticlesRepository
    {
        /// <summary>
        /// <see cref="IArticlesRepository.GetArticlesByNamePatternOrderByName"/>
        /// </summary>
        public IOrderedQueryable<Article> GetArticlesByNamePatternOrderByName(string beginString)
        {
            return string.IsNullOrEmpty(beginString) ? Context.Articles.Include("Authors").Where(a => a.Published).OrderBy(a => a.DisplayName) : Context.Articles.Include("Authors").Where(a => a.Published && a.DisplayName.Trim().ToUpper().Contains(beginString.Trim().ToUpper())).OrderBy(a => a.DisplayName);
        }

        /// <summary>
        /// <see cref="IArticlesRepository.GetArticlesByNamePatternOrderByModificationDate"/>
        /// </summary>
        public IOrderedQueryable<Article> GetArticlesByNamePatternOrderByModificationDate(string beginString)
        {
            return string.IsNullOrEmpty(beginString) ? Context.Articles.Include("Authors").Where(a => a.Published).OrderByDescending(a => a.LastChangedDate) : Context.Articles.Include("Authors").Where(a => a.Published && a.DisplayName.Trim().ToUpper().Contains(beginString.Trim().ToUpper())).OrderByDescending(a => a.LastChangedDate);
        }

        /// <summary>
        /// <see cref="IArticlesRepository.GetArticlesByNamePatternOrderByCreatedDate"/>
        /// </summary>
        public IOrderedQueryable<Article> GetArticlesByNamePatternOrderByCreatedDate(string beginString)
        {
            return string.IsNullOrEmpty(beginString) ? Context.Articles.Include("Authors").Where(a => a.Published).OrderByDescending(a => a.CreatedDate) : Context.Articles.Include("Authors").Where(a => a.Published && a.DisplayName.Trim().ToUpper().Contains(beginString.Trim().ToUpper())).OrderByDescending(a => a.CreatedDate);
        }

        /// <summary>
        /// <see cref="IArticlesRepository.GetAllPublishedArticles"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Article> GetAllPublishedArticles()
        {
            return Context.Articles.Include("Attachments");
        }

        /// <summary>
        /// <see cref="IArticlesRepository.GetPublishedArticleById"/>
        /// </summary>
        public Article GetPublishedArticleById(int id)
        {
            return Context.Articles.FirstOrDefault(a => a.Id == id && a.Published);
        }

        /// <summary>
        /// <see cref="IArticlesRepository.GetAuthorsWithArticles"/>
        /// </summary>
        public IList<Author> GetAuthorsWithArticles()
        {
            return Context.Authors.Where(a => a.Articles.Any()).ToList();
        }

        /// <summary>
        /// <see cref="IArticlesRepository.GetCategoriesWithArticles"/>
        /// </summary>
        public IList<ArticleCategory> GetCategoriesWithArticles()
        {
            return Context.ArticleCategories.Where(a => a.Articles.Any()).ToList();
        }

        /// <summary>
        /// <see cref="IArticlesRepository.SearchArticleByNamePattern"/>
        /// </summary>
        public IQueryable<Article> SearchArticleByNamePattern(string pattern)
        {
            return Context.Articles.Where(a => a.DisplayName.Contains(pattern));
        }

        /// <summary>
        /// <see cref="IArticlesRepository.SearchArticleByDescriptionPattern"/>
        /// </summary>
        public IList<Article> SearchArticleByDescriptionPattern(string pattern)
        {
            return Context.Articles.Where(a => a.DocumentDescription.Trim().ToUpper().Contains(pattern.Trim().ToUpper())).ToList();
        }

        /// <summary>
        /// <see cref="IArticlesRepository.SearchArticleByCategoryNamePattern"/>
        /// </summary>
        public IList<ArticleCategory> SearchArticleByCategoryNamePattern(string pattern)
        {
            return Context.ArticleCategories.Where(c => c.DisplayName.Trim().ToUpper().Contains(pattern.Trim().ToUpper())).ToList();
        }

        /// <summary>
        /// <see cref="IArticlesRepository.SearchArticleByAuthorNamePattern"/>
        /// </summary>
        public IList<Author> SearchArticleByAuthorNamePattern(string pattern)
        {
            return Context.Authors.Where(a => a.Surname.Trim().ToUpper().Contains(pattern.Trim().ToUpper()) || a.Name.Trim().ToUpper().Contains(pattern.Trim().ToUpper())).ToList();
        }
    }
}
