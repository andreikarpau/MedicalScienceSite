using System.Web.Mvc;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using BTTechnologies.MedScience.MVC.Models;

namespace BTTechnologies.MedScience.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService homeService;

        public HomeController(IHomeService newHomeService)
        {
            homeService = newHomeService;
        }

        public ActionResult Index()
        {
            TilesPageModel model = homeService.GetIndexPageModel();
            return View(model);
        }

        public ActionResult About()
        {
            TilesPageModel model = homeService.GetAboutPageModel();
            return View(model);
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}
