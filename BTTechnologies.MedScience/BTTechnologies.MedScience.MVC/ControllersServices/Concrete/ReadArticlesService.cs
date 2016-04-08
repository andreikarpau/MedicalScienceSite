using System;
using System.Collections.Generic;
using System.Linq;
using BTTechnologies.MedScience.Domain.Abstract;
using BTTechnologies.MedScience.Domain.Entities;
using BTTechnologies.MedScience.MVC.App_LocalResources;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using BTTechnologies.MedScience.MVC.Helpers;
using BTTechnologies.MedScience.MVC.Infrastructure;
using BTTechnologies.MedScience.MVC.Models;

namespace BTTechnologies.MedScience.MVC.ControllersServices.Concrete
{
    public class ReadArticlesService : IReadArticlesService
    {
        private readonly IArticlesRepository articlesRepository;

        public ReadArticlesService(IArticlesRepository newArticlesRepository)
        {
            articlesRepository = newArticlesRepository;
        }

        public ShowAllArticlesModel GetAllArticlesModel(string filterString, int? pageNum, int itemsPerPage, ShowAllArticlesModel.SortingType sorting)
        {
            ShowAllArticlesModel model = new ShowAllArticlesModel();

            IQueryable<Article> filteredArticles;

            switch (sorting)
            {
                case ShowAllArticlesModel.SortingType.ByModificationDate:
                    filteredArticles = articlesRepository.GetArticlesByNamePatternOrderByModificationDate(filterString);
                    break;

                case ShowAllArticlesModel.SortingType.ByCreationDate:
                    filteredArticles = articlesRepository.GetArticlesByNamePatternOrderByCreatedDate(filterString);
                    break;

                default:
                    filteredArticles = articlesRepository.GetArticlesByNamePatternOrderByName(filterString);
                    break;
            }

            model.CurrentPage = (pageNum ?? 0) + 1;
            model.NameFilter = filterString;
            int pageNumber = pageNum == null || pageNum < 0 ? 0: (int)pageNum;

            foreach (Article article in filteredArticles.Skip(pageNumber * itemsPerPage).Take(itemsPerPage).ToList())
            {
                ArticleInfo articleInfo = CreateArticleInfo(article);
                if (articleInfo != null)
                    model.ArticlesInfos.Add(articleInfo);
            }

            model.TotalPagesCount = (int)Math.Ceiling((double)filteredArticles.Count() / ShowAllArticlesModel.ItemsPerPage);
            model.SelectedSortingType = sorting;

            return model;
        }

        public ViewArticleModel GetViewArticleModelById(int id)
        {
            Article article = articlesRepository.GetPublishedArticleById(id);

            if (article == null)
                return new ViewArticleModel();

            ViewArticleModel model = new ViewArticleModel();
            ModelsMapper.CreateNewModelUsingMapping(model, article);

            foreach (DocAttachment docAttachment in article.Attachments)
            {
                model.ArticleAttachments.Add(new AttachmentsModel(docAttachment.DisplayName, docAttachment.AttachmentURL, docAttachment.AttachmentType));
            }

            foreach (ArticleCategory articleCategory in article.Categories)
            {
                model.ArticleCategories.Add(new CategoryModel(articleCategory.DisplayName, articleCategory.Id));
            }

            foreach (Author author in article.Authors)
            {
                model.ArticleAuthors.Add(new AuthorModel(author.Id, ServicesHelper.GetAuthorFullName(author)));
            }

            return model;
        }

        public AllAuthorsModel GetAuthorsWithArticles()
        {
            IList<Author> authors = articlesRepository.GetAuthorsWithArticles();

            AllAuthorsModel model = new AllAuthorsModel(); 
            foreach (Author author in authors)
            {
                model.AllAuthors.Add(new AuthorInfoModel(ServicesHelper.GetAuthorFullName(author), author.Id, author.Degree, author.Articles.Count));
            }

            return model;
        }

        public AllCategoriesModel GetCategoriesWithArticles()
        {
            IList<ArticleCategory> categories = articlesRepository.GetCategoriesWithArticles();

            AllCategoriesModel model = new AllCategoriesModel();
            foreach (ArticleCategory articleCategory in categories)
            {
                model.AllCategories.Add(new ArticleCategoryInfoModel(articleCategory.DisplayName, articleCategory.Id, articleCategory.CategoryDescription, articleCategory.Articles.Count));
            }

            return model;
        }

        public ArticlesListModel GetAuthorArticlesList(int id)
        {
            Author author = articlesRepository.GetAuthorById(id);
            if (author == null)
                return new ArticlesListModel(string.Empty);

            ArticlesListModel model = new ArticlesListModel(ServicesHelper.GetAuthorFullName(author), author.Degree);
            foreach (Article article in author.Articles)
            {
                model.ArticlesInfos.Add(CreateArticleInfo(article));
            }

            return model;
        }

        public ArticlesListModel GetCategoryArticlesList(int id)
        {
            ArticleCategory category = articlesRepository.GetCategoryById(id);
            if (category == null)
                return new ArticlesListModel(string.Empty);

            ArticlesListModel model = new ArticlesListModel(string.Format(MedSiteStrings.AllArtilcesOfCategory, category.DisplayName));
            foreach (Article article in category.Articles)
            {
                model.ArticlesInfos.Add(CreateArticleInfo(article));
            }

            return model;
        }

        public IList<ContentQuickSearchItemModel> GetQuickSearchResults(ContentQuickSearchInputModel input, Func<int, string> getArticleUrl, Func<int, string> getCategoryUrl, Func<int, string> getAuthorUrl)
        {
            IList<ContentQuickSearchItemModel> model = new List<ContentQuickSearchItemModel>();
            if (input == null || string.IsNullOrEmpty(input.Pattern) || input.MaxItemsCount <= 0)
                return model;

            int index = 0;

            IList<Article> articles = articlesRepository.SearchArticleByNamePattern(input.Pattern).ToList();
            foreach (Article article in articles)
                model.Add(new ContentQuickSearchItemModel(article.Id, index++, getArticleUrl(article.Id), string.Format(ContentQuickSearchInputModel.GetArticleSearchName(), article.DisplayName), article.DocumentDescription));

            if (input.MaxItemsCount <= model.Count)
                return model.Take(input.MaxItemsCount).ToList();

            IList<Article> articlesByDescription = articlesRepository.SearchArticleByDescriptionPattern(input.Pattern);
            foreach (Article article in articlesByDescription)
            {
                if (model.All(i => i.Id != article.Id))
                    model.Add(new ContentQuickSearchItemModel(article.Id, index++, getArticleUrl(article.Id), string.Format(ContentQuickSearchInputModel.GetArticleSearchName(), article.DisplayName), article.DocumentDescription));
            }

            if (input.MaxItemsCount <= model.Count)
                return model.Take(input.MaxItemsCount).ToList();

            IList<ArticleCategory> categories = articlesRepository.SearchArticleByCategoryNamePattern(input.Pattern);
            foreach (ArticleCategory articleCategory in categories)
                model.Add(new ContentQuickSearchItemModel(articleCategory.Id, index++, getCategoryUrl(articleCategory.Id), string.Format(ContentQuickSearchInputModel.GetCategorySearchName(), articleCategory.DisplayName)));

            if (input.MaxItemsCount <= model.Count)
                return model.Take(input.MaxItemsCount).ToList();

            IList<Author> authors = articlesRepository.SearchArticleByAuthorNamePattern(input.Pattern);
            foreach (Author author in authors)
                model.Add(new ContentQuickSearchItemModel(author.Id, index++, getAuthorUrl(author.Id), string.Format(ContentQuickSearchInputModel.GetAuthorSearchName(), ServicesHelper.GetAuthorFullName(author))));

            return model.Take(input.MaxItemsCount).ToList();
        }

        public IList<ArticleInfo> GetNewOnSite(int count)
        {
            IList<ArticleInfo> model = new List<ArticleInfo>();

            if (count <= 0)
                return model;

            IQueryable<Article> filteredArticles = articlesRepository.GetArticlesByNamePatternOrderByName(string.Empty).OrderByDescending(a => a.CreatedDate).Take(count);
            foreach (Article filteredArticle in filteredArticles)
            {
                ArticleInfo articleInfo = CreateArticleInfo(filteredArticle);
                if (articleInfo == null)
                    continue;

                articleInfo.DocumentDescription = articleInfo.DocumentDescription ?? string.Empty;
                model.Add(articleInfo);                
            }

            return model;
        }

        private ArticleInfo CreateArticleInfo(Article article)
        {
            ArticleInfo articleInfo = new ArticleInfo();
            if (!ModelsMapper.CreateNewModelUsingMapping(articleInfo, article))
                return null;

            foreach (Author author in article.Authors)
                articleInfo.AddAuthorName(ServicesHelper.GetAuthorFullName(author));

            return articleInfo;
        }
    }
}