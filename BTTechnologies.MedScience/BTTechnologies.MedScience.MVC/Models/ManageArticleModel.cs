using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using BTTechnologies.MedScience.MVC.App_LocalResources;
using BTTechnologies.MedScience.MVC.BTTClasses;
using BTTechnologies.MedScience.MVC.QuickSearchString;

namespace BTTechnologies.MedScience.MVC.Models
{
    public class ManageArticleModel: IOkErrorModel
    {
        public ManageArticleModel()
        {
            ArticleAttachments = new List<ArticleAttachmentModel>();
            ArticleAuthors = new List<ArticleAuthorModel>();
            ArticleCategories = new List<ArticleCategoryModel>();
        }

        public int Id { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "ArticleName")]
        [Required(ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "FieldShouldBeFilled")]
        public string DisplayName { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "ArticleDescription")]
        public string DocumentDescription { get; set; }

        [AllowHtml]
        [Display(ResourceType = typeof(MedSiteStrings), Name = "ArticleContent")]
        public string Content { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "Published")]
        public bool Published { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "LastChanged")]
        public DateTime LastChangedDate { get; set; }

        public bool? OperationSucceed { get; set; }

        public QuickSearchStringModel AuthorQuickSearchData { get; set; }

        public QuickSearchStringModel CategoryQuickSearchData { get; set; }

        public IList<ArticleAttachmentModel> ArticleAttachments { get; private set; }

        public IList<ArticleAuthorModel> ArticleAuthors { get; private set; }

        public IList<ArticleCategoryModel> ArticleCategories { get; private set; } 
        
        public IEnumerable<int> CategoriesIds { get; set; }  

        public IEnumerable<int> AuthorsIds { get; set; } 
        
        public IEnumerable<int> DocumentsIds { get; set; } 

        public IEnumerable<string> UploadedFilesGuids { get; set; }
    }

    public class ArticleCategoryModel
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }

        public ArticleCategoryModel(int id, string name)
        {
            Id = id;
            DisplayName = name;
        }
    }

    public class ArticleAttachmentModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }

        public ArticleAttachmentModel(int id, string fileName, string fileUrl)
        {
            Id = id;
            FileName = fileName;
            FileUrl = fileUrl;
        }
    }

    public class ArticleAuthorModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }

        public ArticleAuthorModel(int id, string fullName)
        {
            Id = id;
            FullName = fullName;
        }
    }

    public class QuickAddCategoryModel
    {
        public string DisplayName { get; set; }
        public string CategoryDescription { get; set; }
    }

    public class SiteFilesModel : BTTAjaxGridModel, IOkErrorModel
    {
        public SiteFilesModel(string tableName, string dataActionUrl): base(tableName, dataActionUrl)
        {
        }

        public bool? OperationSucceed { get; set; }
    }

    public class UploadedFiles
    {
        public IEnumerable<string> UploadedFilesGuids { get; set; }
    }
}