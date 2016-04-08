using System;
using System.Collections.Generic;
using System.Linq;
using BTTechnologies.MedScience.Domain.Abstract;
using BTTechnologies.MedScience.Domain.Entities;
using BTTechnologies.MedScience.MVC.App_LocalResources;
using BTTechnologies.MedScience.MVC.BTTClasses;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using BTTechnologies.MedScience.MVC.Helpers;
using BTTechnologies.MedScience.MVC.Infrastructure;
using BTTechnologies.MedScience.MVC.Infrastructure.Messages;
using BTTechnologies.MedScience.MVC.Models;
using BTTechnologies.MedScience.MVC.QuickSearchString;

namespace BTTechnologies.MedScience.MVC.ControllersServices.Concrete
{
    public class AuthorService: IAuthorService
    {
        private readonly IList<BTTAjaxGridHelper.ChangeValueInformation> valuesToChange = ServicesHelper.GetGridChangeBoolValueList();
        private readonly IAuthorsRepository authorsRepository;

        public AuthorService(IAuthorsRepository newAuthorsRepository)
        {
            authorsRepository = newAuthorsRepository;
        }

        public bool CheckIfLinkedAuthorLoginIsEuqalToLogin(int authorId, string userLogin)
        {
            if (string.IsNullOrEmpty(userLogin))
                return false;

            Author author = authorsRepository.GetAuthorById(authorId);

            if (author == null || author.Account == null)
                return false;

            return userLogin.Trim().Equals(author.Account.UserLogin.Trim());
        }

        public BTTAjaxOutputGridModel GetAuthorsData(BTTAjaxInputGridModel inputGridModel)
        {
            return BTTAjaxGridHelper.GetGridData(authorsRepository.Context.AuthorsStatisticsRecords, inputGridModel, valuesToChange);
        }

        public IDictionary<int, string> GetAllFreeAccountsIdsAndNames()
        {
            IDictionary<int, string> accounts = new Dictionary<int, string>();
            foreach (Account account in authorsRepository.GetAccountsWithoutAuthors())
            {
                if (string.IsNullOrEmpty(account.Name) && string.IsNullOrEmpty(account.Surname))
                {
                    accounts.Add(account.Id, account.UserLogin);
                }
                else
                {
                    accounts.Add(account.Id, string.Format(MedSiteStrings.AccountNameSurnameLogin, account.Name, account.Surname, account.UserLogin));
                }
            }

            return accounts;
        }

        public IDictionary<int, string> GetAllFreeAccountsIdsAndNamesIncludingCurrentLinkedAccount(int authorId)
        {
            IDictionary<int, string> accounts = GetAllFreeAccountsIdsAndNames();
            Account account = authorsRepository.GetAccountByAuthorId(authorId);

            if (account != null)
                accounts.Add(account.Id, GetAccountNameInformation(account));

            return accounts;
        }

        public MedScienceErrors AddEditAuthor(AddEditAuthorModel model, string login, out int authorId)
        {
            authorId = model.Id;

            if (!CheckIfLinkedAuthorLoginIsEuqalToLogin(model.Id, login))
                return MedScienceErrors.NonAdminCannotManageNotLinkedAuthor;

            return AddEditAuthorByAdmin(model, out authorId);
        }

        public MedScienceErrors AddEditAuthorByAdmin(AddEditAuthorModel model, out int authorId)
        {
            Author author = null;
            authorId = model.Id;

            if (model.Id != 0)
            {
                author = authorsRepository.GetAuthorById(model.Id);
            }

            if (author == null)
                author = new Author();

            if (!ModelsMapper.CreateNewModelUsingMapping(author, model))
                return MedScienceErrors.UnknownError;

            try
            {
                authorsRepository.AddOrUpdateAuthor(author);
                authorId = author.Id;
            }
            catch (Exception e)
            {
                ExceptionsLogger.LogException(e);
                return MedScienceErrors.UnknownError;
            }

            return MedScienceErrors.NoError;
        }

        public MedScienceErrors QuicklyAddAuthor(QuickAddAuthorModel authorModel, out int authorId)
        {
            authorId = 0;
            AddEditAuthorModel model = new AddEditAuthorModel();

            if (!ModelsMapper.CreateNewModelUsingMapping(model, authorModel))
                return MedScienceErrors.UnknownError;

            return AddEditAuthorByAdmin(model, out authorId);
        }

        public AddEditAuthorModel GetAuthorModelById(int id)
        {
            AddEditAuthorModel model = new AddEditAuthorModel();
            Author author = authorsRepository.GetAuthorById(id);
            if (author == null)
                return model;

            if (!ModelsMapper.CreateNewModelUsingMapping(model, author))
                return new AddEditAuthorModel();

            model.AccountIdsAndNames = GetAllFreeAccountsIdsAndNamesIncludingCurrentLinkedAccount(model.Id);
            model.AccountName = author.Account == null ? MedSiteStrings.NoAccount: GetAccountNameInformation(author.Account); 
            return model;
        }

        public string GetLinkedAccountNameInformationByAuthorId(int id)
        {
            Author author = authorsRepository.GetAuthorById(id);
            if (author == null || author.Account == null)
                return MedSiteStrings.NoAccount;

            return GetAccountNameInformation(author.Account);
        }

        public MedScienceErrors RemoveAuthor(int id)
        {
            Author author = authorsRepository.GetAuthorById(id);

            if (author == null)
                return MedScienceErrors.UnknownError;

            try
            {
                authorsRepository.RemoveAuthor(author);
            }
            catch (Exception e)
            {
                ExceptionsLogger.LogException(e);
                return MedScienceErrors.UnknownError;
            }

            return MedScienceErrors.NoError;
        }

        public string GetAuthorFullNameById(int id)
        {
            Author author = authorsRepository.GetAuthorById(id);

            if (author == null)
                return string.Empty;

            return ServicesHelper.GetAuthorFullName(author);
        }

        public QuickSearchStringOutputModel QuickSearchForAuthor(QuickSearchInputModel inputModel)
        {
            QuickSearchStringOutputModel outputModel = new QuickSearchStringOutputModel();
            IList<Author> authors = authorsRepository.GetAuthorByNameSurnamePatronymicPart(inputModel.CurrentValue);

            foreach (QuickSearchItemInfo authorInfo in from author in authors let fullName = ServicesHelper.GetAuthorFullName(author) select new QuickSearchItemInfo(fullName, author.Degree, author.Id))
            {
                outputModel.SearchResults.Add(authorInfo);
            }

            return outputModel;
        }


        private static string  GetAccountNameInformation(Account account)
        {
            if (account == null)
                return string.Empty;

            if (string.IsNullOrEmpty(account.Name) && string.IsNullOrEmpty(account.Surname))
            {
                return account.UserLogin;
            }

            return string.Format(MedSiteStrings.AccountNameSurnameLogin, account.Name, account.Surname, account.UserLogin);
        }
    }

}