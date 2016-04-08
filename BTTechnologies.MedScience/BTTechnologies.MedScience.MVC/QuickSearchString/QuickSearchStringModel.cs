using BTTechnologies.MedScience.MVC.App_LocalResources;

namespace BTTechnologies.MedScience.MVC.QuickSearchString
{
    public class QuickSearchStringModel
    {
        public string SearchMethodUrl { get; set; }
        public string NothingFoundString { get; set; }
        public string QuickSearchInitString { get; set; }
        public bool NothingFoundIsClickable { get; set; }

        public QuickSearchStringModel()
        {
            NothingFoundString = MedSiteStrings.NothingFound;
        }
    }
}