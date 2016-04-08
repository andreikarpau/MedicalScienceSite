using System.Security;
using System.Web.Mvc;
using BTTechnologies.MedScience.MVC.App_LocalResources;
using BTTechnologies.MedScience.MVC.BTTClasses;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using BTTechnologies.MedScience.MVC.Infrastructure;
using BTTechnologies.MedScience.MVC.Infrastructure.Messages;
using BTTechnologies.MedScience.MVC.Infrastructure.Security;
using BTTechnologies.MedScience.MVC.Models;

namespace BTTechnologies.MedScience.MVC.Controllers
{
    [Permission]
    public class AuthorController : Controller
    {
        private readonly IAuthorService authorService;
        private readonly IMembershipService membershipService;

        public AuthorController(IAuthorService newAuthorService, IMembershipService newMembershipService)
        {
            authorService = newAuthorService;
            membershipService = newMembershipService;
        }

        [Permission(Privilege.SeeAllAuthorsPrivilege)]
        public ActionResult AuthorsGrid()
        {
            BTTAjaxGridModel model = new BTTAjaxGridModel("AuthorGrid", Url.Action("GetAuthorsData", "Author"))
                                         {
                                             AddDeleteColumn = true,
                                             AddEditColumn = true,
                                             DeleteConfirmationText = MedSiteStrings.WantToDeleteRecordQuestion,
                                             DeleteRowUrl = Url.Action("DeleteAuthor", "Author"),
                                             EditRowUrl = Url.Action("EditAuthor", "Author"),
                                             DeleteTooltipText = MedSiteStrings.DeleteText,
                                             EditTooltipText = MedSiteStrings.EditText
                                         };

            model.Columns.Add(new BTTGridColumn { DisplayName = "", ColumnIdentifier = "Id", IsSortable = false, IsHidden = true, IsKey = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.Surname, ColumnIdentifier = "Surname", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.Name, ColumnIdentifier = "Name", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.AuthorDegree, ColumnIdentifier = "Degree", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.AuthorIsActivated, ColumnIdentifier = "AuthorStatus", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.DocumentsCount, ColumnIdentifier = "ArticlesCount", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.LinkedAccountLogin, ColumnIdentifier = "LinkedLogin", IsSortable = true });
            
            return View(model);
        }

        [HttpPost]
        [Permission(Privilege.SeeAllAuthorsPrivilege)]
        public RedirectResult AuthorsGrid(object o)
        {
            return new RedirectResult(Url.Action("AddNewAuthorByAdmin")); 
        }

        [Permission(Privilege.ManageAuthorsPrivilege)]
        public ActionResult EditAuthorByAdmin(int id)
        {
            return View("AddEditAuthor", authorService.GetAuthorModelById(id));
        }

        [Permission(Privilege.ManageAuthorYourselfPrivilege)]
        public ActionResult EditAuthorByUser(int id)
        {
            if (membershipService.UserHasPrivilege(HttpContext, Privilege.ManageAuthorsPrivilege))
                return RedirectToAction("EditAuthorByAdmin", new { id });

            if (!authorService.CheckIfLinkedAuthorLoginIsEuqalToLogin(id, HttpContext.User.Identity.Name))
            {
                throw new SecurityException("User cannot manage data of not linked author");
            }

            return View("AddEditAuthor", authorService.GetAuthorModelById(id));
        }

        [Permission(Privilege.AddAuthorsPrivilege)]
        public ActionResult AddNewAuthorByAdmin()
        {
            return View("AddEditAuthor", new AddEditAuthorModel { AuthorStatus = true, AccountIdsAndNames = authorService.GetAllFreeAccountsIdsAndNames(), AccountName = MedSiteStrings.NoAccount });
        }

        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        [Permission(Privilege.ManageAuthorsPrivilege, Privilege.ManageAuthorYourselfPrivilege)]
        public ActionResult AddEditAuthor(AddEditAuthorModel model)
        {
            MedScienceErrors error = MedScienceErrors.UnknownError;
            if (ModelState.IsValid)
            {
                model.OperationSucceed = true;
                int id;

                error = membershipService.UserHasPrivilege(HttpContext, Privilege.ManageAuthorsPrivilege) ?
                    authorService.AddEditAuthorByAdmin(model, out id) : authorService.AddEditAuthor(model, HttpContext.User.Identity.Name, out id);
                model.Id = id;
            }

            if (error != MedScienceErrors.NoError)
            {
                model.OperationSucceed = false;
                ModelState.AddModelError(string.Empty, EnumDescriptionHelper.GetEnumDescription(error));
            }

            model.AccountIdsAndNames = authorService.GetAllFreeAccountsIdsAndNamesIncludingCurrentLinkedAccount(model.Id);
            model.AccountName = authorService.GetLinkedAccountNameInformationByAuthorId(model.Id);
            
            return View(model);
        }

        [Permission(Privilege.SeeAllAuthorsPrivilege)]
        public JsonResult GetAuthorsData(BTTAjaxInputGridModel inputGridModel)
        {
            BTTAjaxOutputGridModel data = authorService.GetAuthorsData(inputGridModel);
            JsonResult result = Json(data);
            return result;
        }

        [Permission(Privilege.DeleteAuthorPrivilege)]
        public JsonResult DeleteAuthor(int rowId)
        {
            MedScienceErrors error = authorService.RemoveAuthor(rowId);
            return Json(error == MedScienceErrors.NoError ? string.Empty : EnumDescriptionHelper.GetEnumDescription(error));
        }

        [Permission(Privilege.ManageAuthorsPrivilege)]
        public JsonResult EditAuthor(int rowId)
        {
            return Json(new { manageUrl = Url.Action("EditAuthorByAdmin", "Author", new { id = rowId }) });
        }

        [Permission(Privilege.AddAuthorsPrivilege)]
        public JsonResult QuickAddAuthor(QuickAddAuthorModel model)
        {
            int newId;
            MedScienceErrors error = authorService.QuicklyAddAuthor(model, out newId);
            string errorDescription = EnumDescriptionHelper.GetEnumDescription(error);
            return Json(new { newId, errorDescription });
        }
    }
}
