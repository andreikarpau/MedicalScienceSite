using System.Collections.Generic;
using BTTechnologies.MedScience.Domain.Entities;
using BTTechnologies.MedScience.MVC.Infrastructure.Messages;
using BTTechnologies.MedScience.MVC.Models;

namespace BTTechnologies.MedScience.MVC.ControllersServices.Abstract
{
    public interface IAccountService
    {
        IList<string> GetRolesCodesByUserLogin(string login);
        Account GetAccountByUserLogin(string login);
        void UpdateLastLogInDate(Account account);
        bool AccountLoginIsFree(string login);
        MedScienceErrors CreateNewAccount(string login, string password);
        void ActivateUser(int id);
        ManageAccountModel GetUserAccountModel(string login);
        ManageAccountModel GetUserAccountModel(int id);
        bool SaveUserAccount(ManageAccountModel model, string currentUserName);
        ChangeLoginModel GetChangeLoginModel(string login);
        MedScienceErrors ChangeAccountLogin(int id, string newLogin);
        ChangeUserPasswordModel GetCurrentUserPasswordModel(string login);
        MedScienceErrors ChangeUserPassword(int id, string oldPassword, string newPassword);
        MedScienceErrors ChangeUserPassword(string login, string newPassword);
        int? GetAccountId(string login);
        RestorePasswordModel GetRestorePaswordModelById(int id);
        string GetUserLoginById(int id);
        void AddAccountActivationCode(Account account, string code);
        bool VerifyActivationCode(int id, string code);
    }
}
