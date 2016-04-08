using System.Collections.Generic;

namespace BTTechnologies.MedScience.MVC.QuickSearchString
{
    public class QuickSearchStringOutputModel
    {
        public QuickSearchStringOutputModel()
        {
            SearchResults = new List<QuickSearchItemInfo>();
        }

        public IList<QuickSearchItemInfo> SearchResults { get; private set; }
    }

    public class QuickSearchItemInfo
    {
        public QuickSearchItemInfo(string title, string description, string url)
            : this(title, description, url, 0)
        {
        }

        public QuickSearchItemInfo(string title, string description, int id)
            : this(title, description, string.Empty, id)
        {
        }


        public QuickSearchItemInfo(string title, string description, string url, int id)
        {
            Title = title;
            Description = description;
            Url = url;
            Id = id;
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public int Id { get; set; }
    }
}