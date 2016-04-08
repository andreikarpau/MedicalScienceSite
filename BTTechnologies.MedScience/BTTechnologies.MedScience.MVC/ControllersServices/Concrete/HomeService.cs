using System.Linq;
using BTTechnologies.MedScience.Domain.Abstract;
using BTTechnologies.MedScience.Domain.Entities;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using BTTechnologies.MedScience.MVC.Models;

namespace BTTechnologies.MedScience.MVC.ControllersServices.Concrete
{
    public class HomeService : IHomeService
    {
        private readonly IReadOnlyDataRepository readOnlyDataRepository;

        public HomeService(IReadOnlyDataRepository newReadOnlyDataRepository)
        {
            readOnlyDataRepository = newReadOnlyDataRepository;
        }

        public TilesPageModel GetIndexPageModel()
        {
            return new TilesPageModel { PageTiles = readOnlyDataRepository.GetPageTilesByPageType(PageTypes.MainPage).ToList() };
        }

        public TilesPageModel GetAboutPageModel()
        {
            return new TilesPageModel { PageTiles = readOnlyDataRepository.GetPageTilesByPageType(PageTypes.AboutPage).ToList() };
        }
    }
}