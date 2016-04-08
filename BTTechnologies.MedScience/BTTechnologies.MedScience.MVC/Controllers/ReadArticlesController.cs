using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using BTTechnologies.MedScience.MVC.Models;

namespace BTTechnologies.MedScience.MVC.Controllers
{
    public class ReadArticlesController : Controller
    {
        private readonly IReadArticlesService readArticlesService;

        public ReadArticlesController(IReadArticlesService newReadArticlesService)
        {
            readArticlesService = newReadArticlesService;
        }

        public ActionResult ShowAllArticlesList(int? id, string value, string value2)
        {
            ShowAllArticlesModel.SortingType sorting;
            if (!Enum.TryParse(value2, out sorting))
                sorting = ShowAllArticlesModel.SortingType.ByCreationDate;

            return View(readArticlesService.GetAllArticlesModel(value, id != null ? id - 1 : 0, ShowAllArticlesModel.ItemsPerPage, sorting));
        }

        [HttpPost]
        public RedirectResult ShowAllArticlesList(ShowAllArticlesModel model)
        {
            return new RedirectResult(Url.Action("ShowAllArticlesList", new { id = 1, value = model.NameFilter, value2 = model.SelectedSortingType }));
        }

        public ActionResult ShowAllAuthor()
        {
            return View(readArticlesService.GetAuthorsWithArticles());
        }

        public ActionResult ShowAllCategories()
        {
            return View(readArticlesService.GetCategoriesWithArticles());
        }
        
        public ActionResult ShowAuthorArticles(int id)
        {
            return View("ShowArticlesList", readArticlesService.GetAuthorArticlesList(id));
        }        

        public ActionResult ShowCategoryArticles(int id)
        {
            return View("ShowArticlesList", readArticlesService.GetCategoryArticlesList(id));
        }

        public ActionResult ShowArticleView(int id)
        {
            return View(readArticlesService.GetViewArticleModelById(id));
        }

        public ActionResult SearchResults()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SiteQuickSearch(ContentQuickSearchInputModel model)
        {
            IList<ContentQuickSearchItemModel> resultModel = readArticlesService.GetQuickSearchResults(model,
                i => Url.Action("ShowArticleView", new {id = i}),
                i => Url.Action("ShowCategoryArticles", new {id = i}),
                i => Url.Action("ShowAuthorArticles", new {id = i}));

            return Json(resultModel.OrderBy(i => i.Index));
        }

        [HttpPost]
        public JsonResult GetNewOnTheSite(int count)
        {
            return Json(readArticlesService.GetNewOnSite(count));
        }
    }
}
