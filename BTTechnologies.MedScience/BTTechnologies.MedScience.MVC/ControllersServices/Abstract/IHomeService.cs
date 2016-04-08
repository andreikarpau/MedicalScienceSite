using BTTechnologies.MedScience.MVC.Models;

namespace BTTechnologies.MedScience.MVC.ControllersServices.Abstract
{
    public interface IHomeService
    {
        TilesPageModel GetIndexPageModel();
        TilesPageModel GetAboutPageModel();
    }
}