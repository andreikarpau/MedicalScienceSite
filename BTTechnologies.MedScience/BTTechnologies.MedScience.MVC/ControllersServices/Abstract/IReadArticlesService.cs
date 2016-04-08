using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BTTechnologies.MedScience.MVC.Models;

namespace BTTechnologies.MedScience.MVC.ControllersServices.Abstract
{
    public interface IReadArticlesService
    {
        /// <summary>
        /// Get articles model using display name begin filter. (zero based paging)
        /// </summary>
        ShowAllArticlesModel GetAllArticlesModel(string filterString, int? pageNum, int itemsPerPage, ShowAllArticlesModel.SortingType sorting);

        /// <summary>
        /// Get article by id
        /// </summary>
        ViewArticleModel GetViewArticleModelById(int id);

        /// <summary>
        /// Get all authors with articles
        /// </summary>
        AllAuthorsModel GetAuthorsWithArticles();

        /// <summary>
        /// Get all categories with articles
        /// </summary>
        AllCategoriesModel GetCategoriesWithArticles();

        /// <summary>
        /// Get all author articles
        /// </summary>
        ArticlesListModel GetAuthorArticlesList(int id);

        /// <summary>
        /// Get all category articles
        /// </summary>
        ArticlesListModel GetCategoryArticlesList(int id);

        /// <summary>
        /// Search for articles, categories and authors by input information
        /// </summary>
        IList<ContentQuickSearchItemModel> GetQuickSearchResults(ContentQuickSearchInputModel input,
            Func<int, string> getArticleUrl, Func<int, string> getCategoryUrl, Func<int, string> getAuthorUrl);

        IList<ArticleInfo> GetNewOnSite(int count);
    }
}