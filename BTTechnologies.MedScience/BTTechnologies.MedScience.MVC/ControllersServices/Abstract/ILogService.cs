using BTTechnologies.MedScience.MVC.BTTClasses;

namespace BTTechnologies.MedScience.MVC.ControllersServices.Abstract
{
    public interface ILogService
    {
        BTTAjaxOutputGridModel GetArticleLogData(BTTAjaxInputGridModel inputGridModel);
    }
}
