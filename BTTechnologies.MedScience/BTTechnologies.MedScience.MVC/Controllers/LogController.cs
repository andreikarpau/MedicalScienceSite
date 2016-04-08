using System.Web.Mvc;
using BTTechnologies.MedScience.MVC.App_LocalResources;
using BTTechnologies.MedScience.MVC.BTTClasses;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using BTTechnologies.MedScience.MVC.Infrastructure.Security;

namespace BTTechnologies.MedScience.MVC.Controllers
{
    public class LogController : Controller
    {
        private readonly ILogService logService;

        public LogController(ILogService newLogService)
        {
            logService = newLogService;
        }

        [Permission(Privilege.SeeArticleLogPrivilege)]
        public ActionResult ArticleLogGrid()
        {
            BTTAjaxGridModel model = new BTTAjaxGridModel("DocumentsGrid", Url.Action("GetArticleLogData", "Log"))
            {
                AddDeleteColumn = false,
                AddEditColumn = false,
            };

            model.Columns.Add(new BTTGridColumn { DisplayName = "", ColumnIdentifier = "Id", IsSortable = false, IsHidden = true, IsKey = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.Name, ColumnIdentifier = "ItemName", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.Id, ColumnIdentifier = "ItemId", IsSortable = true });

            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.Authors, ColumnIdentifier = "AccountId", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.LoginWhoChanged, ColumnIdentifier = "LoginWhoChanged", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.DBUser, ColumnIdentifier = "DBUser", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.Information, ColumnIdentifier = "ChangesInformation", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.ChangedTime, ColumnIdentifier = "LogDateTime", IsSortable = true });

            return View(model);
        }

        [Permission(Privilege.SeeArticleLogPrivilege)]
        public JsonResult GetArticleLogData(BTTAjaxInputGridModel inputGridModel)
        {
            BTTAjaxOutputGridModel data = logService.GetArticleLogData(inputGridModel);
            JsonResult result = Json(data);
            return result;
        }
    }
}
