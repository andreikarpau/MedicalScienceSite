using System;
using System.Collections.Generic;

namespace BTTechnologies.MedScience.MVC.Models
{
    public class ShowAllArticlesModel
    {
        public enum SortingType
        {
            ByTitle,
            ByModificationDate,
            ByCreationDate
        }

        public const int ItemsPerPage = 20;

        public ShowAllArticlesModel()
        {
            ArticlesInfos = new List<ArticleInfo>();
            CurrentPage = 1;
        }

        public IList<ArticleInfo> ArticlesInfos { get; set; }

        public SortingType SelectedSortingType { get; set; }

        public int TotalPagesCount { get; set; }
     
        public int CurrentPage { get; set; }

        public string NameFilter { get; set; }
    }

    public class ArticleInfo
    {
        public ArticleInfo()
        {
            AuthorsNames = string.Empty;
        }

        public int Id { get; set; }

        public string DisplayName { get; set; }
        
        public string DocumentDescription { get; set; }

        public DateTime LastChangedDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public string AuthorsNames { get; set; }

        public void AddAuthorName(string name)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                return;

            AuthorsNames = string.IsNullOrEmpty(AuthorsNames) ? name.Trim() : string.Format("{0}, {1}", AuthorsNames, name.Trim());
        }
    }

    public class ViewArticleModel
    {
        public ViewArticleModel()
        {
            ArticleAttachments = new List<AttachmentsModel>();
            ArticleCategories = new List<CategoryModel>();
            ArticleAuthors = new List<AuthorModel>();
        }

        /// <summary>
        /// Article id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Document display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Document description
        /// </summary>
        public string DocumentDescription { get; set; }

        /// <summary>
        /// Document content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Shows if article is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Date and time when article was last changed
        /// </summary>
        public DateTime LastChangedDate { get; set; }

        public IList<AttachmentsModel> ArticleAttachments { get; private set; }

        public IList<CategoryModel> ArticleCategories { get; private set; }

        public IList<AuthorModel> ArticleAuthors { get; private set; }
    }

    public class AttachmentsModel
    {
        public string AttachmentDisplayName { get; set; }
        public string AttachmentType { get; set; }
        public string AttachmentUrl { get; set; }

        public AttachmentsModel(string displayName, string url, string attachmentType)
        {
            AttachmentDisplayName = displayName;
            AttachmentUrl = url;
            AttachmentType = attachmentType;
        }

        public string GetFullFileName()
        {
            if (string.IsNullOrEmpty(AttachmentDisplayName))
                return string.Empty;

            if (string.IsNullOrEmpty(AttachmentType))
                return AttachmentDisplayName;

            return string.Format("{0}{1}", AttachmentDisplayName, AttachmentType);
        }
    }

    public class CategoryModel
    {
        public string CategoryDisplayName { get; set; }
        public int CategoryId { get; set; }
    
        public CategoryModel(string displayName, int id)
        {
            CategoryDisplayName = displayName;
            CategoryId = id;
        }
    }

    public class AuthorModel
    {
        public int AuthorId { get; set; }
        public string FullAuthorName { get; set; }

        public AuthorModel(int id, string fullName)
        {
            AuthorId = id;
            FullAuthorName = fullName;
        }
    }

    public class AllAuthorsModel
    {
        public IList<AuthorInfoModel> AllAuthors { get; private set; }

        public AllAuthorsModel()
        {
            AllAuthors = new List<AuthorInfoModel>();
        }
    }

    public class AuthorInfoModel
    {
        public string FullName { get; set; }
        public int AuthorId { get; set; }
        public string AuthorDegree { get; set; }
        public int ArticlesCount { get; set; }

        public AuthorInfoModel(string fullName, int authorId, string authorDegree, int articlesCount)
        {
            FullName = fullName;
            AuthorDegree = authorDegree;
            AuthorId = authorId;
            ArticlesCount = articlesCount;
        }
    }

    public class AllCategoriesModel
    {
        public IList<ArticleCategoryInfoModel> AllCategories { get; private set; }

        public AllCategoriesModel()
        {
            AllCategories = new List<ArticleCategoryInfoModel>();
        }
    }

    public class ArticleCategoryInfoModel
    {
        public string DisplayName { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public int CategoryArticlesCount { get; set; }

        public ArticleCategoryInfoModel(string displayName, int categoryId, string description, int articlesCount)
        {
            DisplayName = displayName;
            CategoryId = categoryId;
            Description = description;
            CategoryArticlesCount = articlesCount;
        }
    }

    public class ArticlesListModel
    {
        public IList<ArticleInfo> ArticlesInfos { get; private set; }

        public string HeaderText { get; private set; }
        public string DescriptionText { get; private set; }

        public ArticlesListModel(string headerText)
        {
            HeaderText = headerText;
            ArticlesInfos = new List<ArticleInfo>();
        }

        public ArticlesListModel(string headerText, string descriptionText): this(headerText)
        {
            DescriptionText = descriptionText;
        }
    }
}