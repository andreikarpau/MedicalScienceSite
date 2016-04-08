using System.Collections.Generic;
using BTTechnologies.MedScience.MVC.BTTClasses;
using BTTechnologies.MedScience.MVC.Infrastructure.Messages;
using BTTechnologies.MedScience.MVC.Models;
using BTTechnologies.MedScience.MVC.QuickSearchString;

namespace BTTechnologies.MedScience.MVC.ControllersServices.Abstract
{
    public interface IAuthorService
    {
        BTTAjaxOutputGridModel GetAuthorsData(BTTAjaxInputGridModel inputGridModel);
        IDictionary<int, string> GetAllFreeAccountsIdsAndNames();
        IDictionary<int, string> GetAllFreeAccountsIdsAndNamesIncludingCurrentLinkedAccount(int authorId);
        MedScienceErrors AddEditAuthorByAdmin(AddEditAuthorModel model, out int authorId);
        MedScienceErrors AddEditAuthor(AddEditAuthorModel model, string login, out int authorId);
        AddEditAuthorModel GetAuthorModelById(int id);
        string GetLinkedAccountNameInformationByAuthorId(int id);
        MedScienceErrors RemoveAuthor(int id);
        bool CheckIfLinkedAuthorLoginIsEuqalToLogin(int authorId, string userLogin);
        MedScienceErrors QuicklyAddAuthor(QuickAddAuthorModel authorModel, out int authorId);
        string GetAuthorFullNameById(int id);
        QuickSearchStringOutputModel QuickSearchForAuthor(QuickSearchInputModel inputModel);
    }
}
