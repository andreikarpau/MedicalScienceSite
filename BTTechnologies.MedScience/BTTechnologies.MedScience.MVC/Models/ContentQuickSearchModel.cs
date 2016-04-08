using BTTechnologies.MedScience.MVC.App_LocalResources;

namespace BTTechnologies.MedScience.MVC.Models
{
    public class ContentQuickSearchItemModel
    {
        public int Id { get; private set; }
        public string ItemUrl { get; private set; }
        public string ItemName { get; private set; }
        public string ItemDescription { get; private set; }
        public int Index { get; private set; }

        public ContentQuickSearchItemModel(int id, int index, string url, string itemName, string itemDescription = null)
        {
            Id = id;
            Index = index;
            ItemUrl = url;
            ItemName = itemName;
            ItemDescription = itemDescription ?? string.Empty;
        }
    }

    public class ContentQuickSearchInputModel
    {
        private static readonly string DefaultArticleSearchName = MedSiteStrings.ArticleSearchName;
        private static readonly string DefaultAuthorSearchName = MedSiteStrings.AuthorSearchName;
        private static readonly string DefaultCategorySearchName = MedSiteStrings.CategorySearchName;
        
        public int MaxItemsCount { get; set; }
        public string Pattern { get; set; }

        public static string GetArticleSearchName()
        {
            return DefaultArticleSearchName;
        }

        public static string GetAuthorSearchName()
        {
            return DefaultAuthorSearchName;
        }

        public static string GetCategorySearchName()
        {
            return DefaultCategorySearchName;
        }
    }
}