using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using BTTechnologies.MedScience.MVC.App_LocalResources;
using BTTechnologies.MedScience.MVC.BTTClasses;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using BTTechnologies.MedScience.MVC.Helpers;
using BTTechnologies.MedScience.MVC.Infrastructure;
using BTTechnologies.MedScience.MVC.Infrastructure.Messages;
using BTTechnologies.MedScience.MVC.Infrastructure.Security;
using BTTechnologies.MedScience.MVC.Models;
using BTTechnologies.MedScience.MVC.QuickSearchString;

namespace BTTechnologies.MedScience.MVC.Controllers
{
    [Permission]
    public class DocumentsController : Controller
    {
        private readonly IDocumentService documentService;
        private readonly IAuthorService authorService;
        private readonly BTTAjaxFileUploader fileUploader = new BTTAjaxFileUploader();
        private readonly string baseDocumentsUploadDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManagerHelper.UploadArticleDocumentsPath);
        private readonly string baseUploadFilesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManagerHelper.UploadFilesPath);

        public DocumentsController(IDocumentService newDocumentsService, IAuthorService newAuthorService)
        {
            documentService = newDocumentsService;
            authorService = newAuthorService;
        }

        [Permission(Privilege.SeeAllDocumentsPrivilege)]
        public ActionResult DocumentsGrid()
        {
            BTTAjaxGridModel model = new BTTAjaxGridModel("DocumentsGrid", Url.Action("GetDocumentsData", "Documents"))
            {
                AddDeleteColumn = true,
                AddEditColumn = true,
                DeleteConfirmationText = MedSiteStrings.WantToDeleteRecordQuestion,
                DeleteRowUrl = Url.Action("DeleteDocument", "Documents"),
                EditRowUrl = Url.Action("EditDocument", "Documents"),
                DeleteTooltipText = MedSiteStrings.DeleteText,
                EditTooltipText = MedSiteStrings.EditText
            };

            model.Columns.Add(new BTTGridColumn { DisplayName = "", ColumnIdentifier = "Id", IsSortable = false, IsHidden = true, IsKey = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.ArticleName, ColumnIdentifier = "DisplayName", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.ArticleDescription, ColumnIdentifier = "DocumentDescription", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.Authors, ColumnIdentifier = "AuthorsNames", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.AttachedFilesCount, ColumnIdentifier = "AttachmentsCount", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.Categories, ColumnIdentifier = "Categories", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.Published, ColumnIdentifier = "Published", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.LastChanged, ColumnIdentifier = "LastChangedDate", IsSortable = true });
            
            return View(model);
        }

        [HttpPost]
        [Permission(Privilege.SeeAllDocumentsPrivilege)]
        public RedirectResult DocumentsGrid(object o)
        {
            return new RedirectResult(Url.Action("AddNewArticle"));
        }

        [Permission(Privilege.AddDocumentPrivilege)]
        public ActionResult AddNewArticle()
        {
            ManageArticleModel newModel = documentService.GetNewArticleModel(Url.Action("AuthorQuickSearch"), Url.Action("CategoryQuickSearch"));
            newModel.Published = ServicesHelper.GetMembershipService().UserHasPrivilege(HttpContext.User.Identity.Name, Privilege.CanPublishArticle);

            return View("ManageArticle", newModel);
        }

        [Permission(Privilege.ManageDocumentPrivilege)]
        public ActionResult ManageArticle(int id)
        {
            return View("ManageArticle", documentService.GetArticleModelById(id, Url.Action("AuthorQuickSearch"), Url.Action("CategoryQuickSearch")));
        }

        [HttpPost]
        [ValidateAntiForgeryTokenAttribute]
        [Permission(Privilege.ManageDocumentPrivilege)]
        public ActionResult ManageArticle(ManageArticleModel model)
        {
            int id = model.Id;
            bool operationSucceed = ModelState.IsValid;

            if (operationSucceed)
            {
                MedScienceErrors error = documentService.AddEditDocument(model, HttpContext.User.Identity.Name, fileUploader, baseDocumentsUploadDirectory, out id);
                if (error != MedScienceErrors.NoError)
                {
                    ModelState.AddModelError(string.Empty, EnumDescriptionHelper.GetEnumDescription(error));
                    operationSucceed = false;
                }
            }
            
            ManageArticleModel newModel = documentService.GetArticleModelById(id, Url.Action("AuthorQuickSearch"), Url.Action("CategoryQuickSearch"));

            if (!operationSucceed)
                CopyManageArticleModelBaseProperties(newModel, model);

            newModel.OperationSucceed = operationSucceed;
            return View(newModel);
        }

        [Permission(Privilege.ManageDocumentPrivilege)]
        public JsonResult EditDocument(int rowId)
        {
            return Json(new { manageUrl = Url.Action("ManageArticle", "Documents", new { id = rowId }) });
        }

        [Permission(Privilege.DeleteDocumentPrivilege)]
        public ActionResult DeleteDocument(int rowId)
        {
            MedScienceErrors error = documentService.RemoveDocument(rowId, baseDocumentsUploadDirectory);
            return Json(error == MedScienceErrors.NoError ? string.Empty : EnumDescriptionHelper.GetEnumDescription(error));
        }

        [HttpPost]
        [Permission(Privilege.SeeAllAuthorsPrivilege)]
        public JsonResult AuthorQuickSearch(QuickSearchInputModel inputModel)
        {
            QuickSearchStringOutputModel outputModel = authorService.QuickSearchForAuthor(inputModel);
            return Json(outputModel);
        }

        [HttpPost]
        public JsonResult CategoryQuickSearch(QuickSearchInputModel inputModel)
        {
            QuickSearchStringOutputModel outputModel = documentService.QuickSearchForCategories(inputModel);
            return Json(outputModel);
        }

        [HttpPost]
        [Permission(Privilege.SeeAllDocumentsPrivilege)]
        public JsonResult GetDocumentsData(BTTAjaxInputGridModel inputGridModel)
        {
            BTTAjaxOutputGridModel data = documentService.GetDocumentsData(inputGridModel);
            JsonResult result = Json(data);
            return result;
        }

        [HttpPost]
        public JsonResult GetAuthorFullNameById(int id)
        {
            string authorUrl = Url.Action("EditAuthorByUser", "Author", new {id});
            return Json(new { fullName = authorService.GetAuthorFullNameById(id), url = authorUrl });
        }

        [HttpPost]
        public JsonResult GetCategoryFullNameById(int id)
        {
            return Json(new { DisplayName = documentService.GetCategoryFullNameById(id) });
        }

        [Permission(Privilege.AddCategoryPrivilege)]
        public JsonResult QuickAddCategory(QuickAddCategoryModel model)
        {
            int newId;
            MedScienceErrors error = documentService.QuicklyAddCategory(model, out newId);
            string errorDescription = EnumDescriptionHelper.GetEnumDescription(error);
            return Json(new { newId, errorDescription });
        }

        [HttpPost]
        [Permission(Privilege.ManageDocumentPrivilege)]
        public JsonResult AddFileToTempFolder()
        {
            BTTAjaxFileUploader saver = new BTTAjaxFileUploader();
            UploadFilesInputModel inputModel = new UploadFilesInputModel();
            
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                inputModel.PostedFiles.Add(file.FileName, file);
            }

            UploadFilesOutputModel outputModel = saver.SaveFilesToTempDirectory(inputModel);
            return Json(outputModel);
        }
        
        [Permission(Privilege.SeeSiteFiles)]
        public ActionResult SiteFilesGrid()
        {
            return View(GetSiteFilesModel());
        }

        [Permission(Privilege.SeeSiteFiles, Privilege.AddSiteFiles)]
        public ActionResult AddSiteFiles(UploadedFiles files)
        {
            MedScienceErrors error = documentService.SaveUploadedFiles(files, baseUploadFilesDirectory, fileUploader);
            SiteFilesModel model = GetSiteFilesModel();
            model.OperationSucceed = error == MedScienceErrors.NoError;
            
            return View("SiteFilesGrid", model);
        }

        [HttpPost]
        [Permission(Privilege.SeeSiteFiles)]
        public JsonResult GetSiteFilesData(BTTAjaxInputGridModel inputGridModel)
        {
            BTTAjaxOutputGridModel data = documentService.GetSiteFilesData(inputGridModel, Url);
            JsonResult result = Json(data);
            return result;
        }

        [Permission(Privilege.DeleteSiteFiles)]
        public ActionResult DeleteSiteFile(int rowId)
        {
            MedScienceErrors error = documentService.RemoveFile(rowId, baseDocumentsUploadDirectory);
            return Json(error == MedScienceErrors.NoError ? string.Empty : EnumDescriptionHelper.GetEnumDescription(error));
        }

        private SiteFilesModel GetSiteFilesModel()
        {
            SiteFilesModel model = new SiteFilesModel("DocumentsGrid", Url.Action("GetSiteFilesData", "Documents"))
            {
                AddDeleteColumn = true,
                DeleteConfirmationText = MedSiteStrings.DeleteFileConfirm,
                DeleteRowUrl = Url.Action("DeleteSiteFile", "Documents"),
                DeleteTooltipText = MedSiteStrings.DeleteText
            };

            model.Columns.Add(new BTTGridColumn { DisplayName = "", ColumnIdentifier = "Id", IsSortable = false, IsHidden = true, IsKey = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.Name, ColumnIdentifier = "DisplayName", IsSortable = true });
            model.Columns.Add(new BTTGridColumn { DisplayName = MedSiteStrings.Url, ColumnIdentifier = "FileUrl", IsSortable = true });

            return model;
        }

        private void CopyManageArticleModelBaseProperties(ManageArticleModel copyTo, ManageArticleModel copyFrom)
        {
            copyTo.DisplayName = copyFrom.DisplayName;
            copyTo.DocumentDescription = copyFrom.DocumentDescription;
            copyTo.Published = copyFrom.Published;
            copyTo.Content = copyFrom.Content;
        }
    }
}
