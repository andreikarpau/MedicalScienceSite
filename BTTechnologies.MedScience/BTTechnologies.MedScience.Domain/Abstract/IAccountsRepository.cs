using System.Collections.Generic;
using BTTechnologies.MedScience.Domain.Context;
using BTTechnologies.MedScience.Domain.Entities;

namespace BTTechnologies.MedScience.Domain.Abstract
{
    /// <summary>
    /// Accounts repository
    /// </summary>
    public interface IAccountsRepository
    {
        /// <summary>
        /// Table context
        /// </summary>
        MedScienceDBContext Context { get; }

        /// <summary>
        /// Get Accounts
        /// </summary>
        /// <returns></returns>
        IList<Account> GetAccounts();

        /// <summary>
        /// Get Accounts
        /// </summary>
        Account GetAccountByUserLogin(string login);

        /// <summary>
        /// Get Accounts
        /// </summary>
        Account GetAccountById(int id);

        /// <summary>
        /// Add new account or updates it if item with the same id already exists
        /// </summary>
        void AddOrUpdateAccount(Account account);

        /// <summary>
        /// Remove account
        /// </summary>
        void RemoveAccount(Account account);

        /// <summary>
        /// Check if such login already exists
        /// </summary>
        bool CheckIfLoginExists(string login);

        /// <summary>
        /// Add roles with codes to account
        /// </summary>
        void AddRolesWithCodes(Account account, params string[] codes);
        
        /// <summary>
        /// Remove roles with codes to account
        /// </summary>
        void RemoveRolesWithCodes(Account account, params string[] codes);

        /// <summary>
        /// Add role with roleId to account
        /// </summary>
        void AddRoleWithId(Account account, int roleId);
        
        /// <summary>
        /// Remove role with roleId from account
        /// </summary>
        void RemoveRoleWithId(Account account, int roleId);

        /// <summary>
        /// Get account id by login
        /// </summary>
        int? GetAccountId(string login);

        /// <summary>
        /// Get account roles names
        /// </summary>
        IList<string> GetAccountRolesNames(int accountId);

        /// <summary>
        /// Get roles
        /// </summary>
        /// <returns></returns>
        IList<Role> GetRoles();

        /// <summary>
        /// Get Role with Code
        /// </summary>
        /// <returns></returns>
        Role GetRoleWithCode(string roleCode);

        /// <summary>
        /// Add new account activation code
        /// </summary>
        void AddActivationCode(AccountActivationCode accountActivationCode);

        /// <summary>
        /// Get if activation code linked to account with Id = accountId exists and delete this code
        /// </summary>
        bool GetIfActivationCodeLinkedToAccountExists(int accountId, string code);

        /// <summary>
        /// Get page tile by id
        /// </summary>
        PageTile GetPageTileById(int id);

        /// <summary>
        /// Add new or edit existing page tile
        /// </summary>
        void AddEditPageTile(PageTile pageTile);

        /// <summary>
        /// Remove page tile
        /// </summary>
        void RemovePageTile(PageTile pageTile);
    }
}
