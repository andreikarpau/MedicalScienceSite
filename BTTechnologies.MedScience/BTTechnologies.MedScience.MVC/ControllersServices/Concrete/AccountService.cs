using System;
using System.Collections.Generic;
using BTTechnologies.MedScience.Domain.Abstract;
using BTTechnologies.MedScience.Domain.Entities;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using System.Linq;
using BTTechnologies.MedScience.MVC.Infrastructure;
using BTTechnologies.MedScience.MVC.Infrastructure.Messages;
using BTTechnologies.MedScience.MVC.Models;

namespace BTTechnologies.MedScience.MVC.ControllersServices.Concrete
{
    public class AccountService: IAccountService
    {
        private readonly IAccountsRepository accountsRepository;

        public AccountService(IAccountsRepository newAccountsRepository)
        {
            accountsRepository = newAccountsRepository;
        }       

        public MedScienceErrors CreateNewAccount(string login, string password)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                return MedScienceErrors.UnknownError;
            
            if (!AccountLoginIsFree(login.Trim()))
                return MedScienceErrors.AccountWithSameLoginAlreadyExistsTryToBackupYouPassword;

            Account account = new Account { UserLogin = login.Trim(), UserPassword = password };
            accountsRepository.AddOrUpdateAccount(account);
            return MedScienceErrors.NoError;
        }

        public MedScienceErrors ChangeAccountLogin(int id, string newLogin)
        {
            if (string.IsNullOrEmpty(newLogin))
                return MedScienceErrors.UnknownError;
            
            if (!AccountLoginIsFree(newLogin.Trim()))
                return MedScienceErrors.AccountWithSameLoginAlreadyExists;

            Account account = accountsRepository.GetAccountById(id);
            if (account == null)
                return MedScienceErrors.UnknownError;

            account.UserLogin = newLogin.Trim();
            accountsRepository.AddOrUpdateAccount(account);
            accountsRepository.RemoveRolesWithCodes(account, Role.USER_ROLE_CODE);
            accountsRepository.AddRolesWithCodes(account, Role.NOT_ACTIVATED_USER_ROLE_CODE);
            return MedScienceErrors.NoError;
        }
        
        public bool AccountLoginIsFree(string login)
        {
            if (string.IsNullOrEmpty(login))
                return true;

            return !accountsRepository.CheckIfLoginExists(login.Trim());
        }

        public IList<string> GetRolesCodesByUserLogin(string login)
        {
            if (string.IsNullOrEmpty(login))
                return new List<string>();

            Account account = accountsRepository.GetAccountByUserLogin(login.Trim());
            if (account == null)
                return new List<string>();

            return (from role in account.Roles select role.Code).ToList();
        }

        public Account GetAccountByUserLogin(string login)
        {
            return login == null ? null : accountsRepository.GetAccountByUserLogin(login.Trim());
        }

        public void UpdateLastLogInDate(Account account)
        {
            account.LastLogInDate = DateTime.Now;
            accountsRepository.AddOrUpdateAccount(account);
        }

        public void ActivateUser(int id)
        {
            Account account = accountsRepository.GetAccountById(id);

            if (account == null)
                return;

            accountsRepository.RemoveRolesWithCodes(account, Role.NOT_ACTIVATED_USER_ROLE_CODE);
            accountsRepository.AddRolesWithCodes(account, Role.USER_ROLE_CODE);
        }

        public ManageAccountModel GetUserAccountModel(string login)
        {
            if (login == null)
                return new ManageAccountModel();

            return GetUserAccountModel(accountsRepository.GetAccountByUserLogin(login));
        }

        public ManageAccountModel GetUserAccountModel(int id)
        {
            if (id <= 0)
                return new ManageAccountModel();

            return GetUserAccountModel(accountsRepository.GetAccountById(id));
        }

        public bool SaveUserAccount(ManageAccountModel model, string currentUserName)
        {
            Account currentUserAccount = accountsRepository.GetAccountByUserLogin(currentUserName);

            if (model == null || model.Id == 0 || currentUserAccount == null || model.Id != currentUserAccount.Id)
                return false;

            Account account = accountsRepository.GetAccountById(model.Id);

            if (account == null)
                return false;

            if (!ModelsMapper.CreateNewModelUsingMapping(account, model))
                return false;

            accountsRepository.AddOrUpdateAccount(account);
            return true;
        }

        public ChangeLoginModel GetChangeLoginModel(string login)
        {
            if (login == null)
                return new ChangeLoginModel();

            Account account = accountsRepository.GetAccountByUserLogin(login.Trim());
            if (account == null)
                return new ChangeLoginModel();

            ChangeLoginModel model = new ChangeLoginModel();
            return ModelsMapper.CreateNewModelUsingMapping(model, account) ? model : new ChangeLoginModel();            
        }

        public ChangeUserPasswordModel GetCurrentUserPasswordModel(string login)
        {
            if (login == null)
                return new ChangeUserPasswordModel();

            int? id = accountsRepository.GetAccountId(login.Trim());
            return id == null ? new ChangeUserPasswordModel() : new ChangeUserPasswordModel { Id = (int)id };
        }

        public MedScienceErrors ChangeUserPassword(int id, string oldPassword, string newPassword)
        {
            Account account = accountsRepository.GetAccountById(id);

            if (account == null || string.IsNullOrEmpty(newPassword))
                return MedScienceErrors.UnknownError;

            if (oldPassword == null || account.UserPassword.Trim() != oldPassword.Trim())
                return MedScienceErrors.IncorrectPassword;

            account.UserPassword = newPassword.Trim();
            accountsRepository.AddOrUpdateAccount(account);
            return MedScienceErrors.NoError;
        }     
   
        public MedScienceErrors ChangeUserPassword(string login, string newPassword)
        {
            Account account = accountsRepository.GetAccountByUserLogin(login);
            if (account == null)
                return MedScienceErrors.UnknownError;

            return ChangeUserPassword(account.Id, account.UserPassword, newPassword);
        }
        
        public int? GetAccountId(string login)
        {
            return accountsRepository.GetAccountId(login);
        }

        public string GetUserLoginById(int id)
        {
            Account account = accountsRepository.GetAccountById(id);
            if (account == null)
                return string.Empty;

            return account.UserLogin;
        }

        public RestorePasswordModel GetRestorePaswordModelById(int id)
        {
            Account account = accountsRepository.GetAccountById(id);
            return account == null ? new RestorePasswordModel() : new RestorePasswordModel {UserLogin = account.UserLogin};
        }

        public void AddAccountActivationCode(Account account, string code)
        {
            AccountActivationCode accountActivationCode = new AccountActivationCode {Account = account, Code = code};
            accountsRepository.AddActivationCode(accountActivationCode);
        }

        public bool VerifyActivationCode(int id, string code)
        {
            return accountsRepository.GetIfActivationCodeLinkedToAccountExists(id, code);
        }

        private ManageAccountModel GetUserAccountModel(Account account)
        {
            if (account == null)
                return new ManageAccountModel();

            ManageAccountModel model = new ManageAccountModel();
            
            if (!ModelsMapper.CreateNewModelUsingMapping(model, account))
                return new ManageAccountModel();

            if (account.Authors != null && account.Authors.Any())
            {
                model.AuthorId = account.Authors.First().Id;
            } 

            return model;
        }
    }
}