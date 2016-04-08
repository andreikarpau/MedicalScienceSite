using BTTechnologies.MedScience.MVC.BTTClasses;

namespace BTTechnologies.MedScience.MVC.Models
{
    public class AllUsersGridModel : BTTAjaxGridModel
    {
        public AllUsersGridModel(string tableName, string dataActionUrl): base(tableName, dataActionUrl)
        {}
    }
}