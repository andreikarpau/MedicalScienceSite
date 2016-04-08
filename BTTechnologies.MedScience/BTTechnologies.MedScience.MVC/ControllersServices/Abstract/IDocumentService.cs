using System.Web.Mvc;
using BTTechnologies.MedScience.MVC.BTTClasses;
using BTTechnologies.MedScience.MVC.Infrastructure.Messages;
using BTTechnologies.MedScience.MVC.Models;
using BTTechnologies.MedScience.MVC.QuickSearchString;

namespace BTTechnologies.MedScience.MVC.ControllersServices.Abstract
{
    public interface IDocumentService
    {
        BTTAjaxOutputGridModel GetDocumentsData(BTTAjaxInputGridModel inputGridModel);
        MedScienceErrors AddEditDocument(ManageArticleModel model, string userLogin, BTTAjaxFileUploader fileUploader, string baseDocumentsPath, out int id);
        QuickSearchStringOutputModel QuickSearchForCategories(QuickSearchInputModel inputModel);
        string GetCategoryFullNameById(int id);
        ManageArticleModel GetNewArticleModel(string authorSearchMethodUrl, string categorySearchMethodUrl);
        ManageArticleModel GetArticleModelById(int id, string authorSearchMethodUrl, string categorySearchMethodUrl);
        MedScienceErrors QuicklyAddCategory(QuickAddCategoryModel model, out int newId);
        MedScienceErrors RemoveDocument(int id, string baseDocumentsUploadDirectory);
        MedScienceErrors RemoveFile(int id, string baseDocumentsUploadDirectory);
        BTTAjaxOutputGridModel GetSiteFilesData(BTTAjaxInputGridModel inputGridModel, UrlHelper urlHelper);
        MedScienceErrors SaveUploadedFiles(UploadedFiles files, string uploadPath, BTTAjaxFileUploader fileUploader);
    }
}
