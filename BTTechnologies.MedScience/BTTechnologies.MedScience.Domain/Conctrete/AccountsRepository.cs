using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using BTTechnologies.MedScience.Domain.Abstract;
using BTTechnologies.MedScience.Domain.Entities;

namespace BTTechnologies.MedScience.Domain.Conctrete
{
    /// <summary>
    /// Accoutns repository
    /// </summary>
    public class AccountsRepository: BaseRepository, IAccountsRepository
    {
        private const int activationWaitTimeInHours = 168;

        /// <summary>
        /// <see cref="IAccountsRepository.GetAccounts"/>
        /// </summary>
        public IList<Account> GetAccounts()
        {
            return Context.Accounts.ToList();
        }

        /// <summary>
        /// <see cref="IAccountsRepository.GetAccountByUserLogin"/>
        /// </summary>
        public Account GetAccountByUserLogin(string login)
        {
            string searchLogin = GetLowerTrimedLogin(login);
            return Context.Accounts.FirstOrDefault(acc => acc.UserLogin.Equals(searchLogin));
        }

        /// <summary>
        /// <see cref="IAccountsRepository.AddOrUpdateAccount"/>
        /// </summary>
        public void AddOrUpdateAccount(Account account)
        {
            account.UserLogin = GetLowerTrimedLogin(account.UserLogin);
            if (account.Id == 0)
            {
                Role role = Context.Roles.FirstOrDefault(r => r.Code == Role.NOT_ACTIVATED_USER_ROLE_CODE);

                if (role == null)
                {
                    Debug.Assert(role == null, "Role should not be null");
                }
                else
                {
                    account.Roles.Add(role);
                }

                account.RegistrationDate = DateTime.Now;
                Context.Accounts.Add(account);
            }
            else
            {
                Context.Entry(account).State = EntityState.Modified;
            }

            Context.SaveChanges();
        }

        /// <summary>
        /// <see cref="IAccountsRepository.RemoveAccount"/>
        /// </summary>
        public void RemoveAccount(Account account)
        {
            if (account == null)
                return;

            string[] codes = account.Roles.Select(r => r.Code).ToArray();
            RemoveRolesWithCodes(account, codes);
            
            IList<AccountActivationCode> activationCodes = Context.AccountActivationCodes.Where(a => a.Account.Id == account.Id).ToList();
            foreach (AccountActivationCode accountActivationCode in activationCodes)
            {
                Context.AccountActivationCodes.Remove(accountActivationCode);
            }


            Context.Accounts.Remove(account);
            Context.SaveChanges();
        }

        /// <summary>
        /// <see cref="IAccountsRepository.CheckIfLoginExists"/>
        /// </summary>
        public bool CheckIfLoginExists(string login)
        {
            string searchLogin = GetLowerTrimedLogin(login);
            return Context.Accounts.Any(acc => acc.UserLogin.Equals(searchLogin));
        }

        /// <summary>
        /// <see cref="IAccountsRepository.AddRolesWithCodes"/>
        /// </summary>
        public void AddRolesWithCodes(Account account, params string[] codes)
        {
            if (account == null || codes == null || !codes.Any())
                return;


            foreach (string code in codes)
            {
                if (account.Roles.Any(r => r.Code == code))
                    continue;

                Role role = Context.Roles.FirstOrDefault(r => r.Code == code);
                
                if (role == null)
                    continue;

                account.Roles.Add(role);
            }

            Context.SaveChanges();
        }

        /// <summary>
        /// <see cref="IAccountsRepository.RemoveRolesWithCodes"/>
        /// </summary>
        public void RemoveRolesWithCodes(Account account, params string[] codes)
        {
            if (account == null || codes == null || !codes.Any())
                return;
            
            foreach (string code in codes)
            {
                string codeValue = code;
                IList<Role> removeRoles = account.Roles.Where(r => r.Code == codeValue).ToList();
                foreach (Role removeRole in removeRoles)
                {
                    account.Roles.Remove(removeRole);
                }
            }

            Context.SaveChanges();
        }


        /// <summary>
        /// <see cref="IAccountsRepository.AddRoleWithId"/>
        /// </summary>
        public void AddRoleWithId(Account account, int roleId)
        {
            Role role = Context.Roles.FirstOrDefault(r => r.Id == roleId);
            if (account == null || role == null || account.Roles.Contains(role))
                return;
            
            account.Roles.Add(role);
            Context.SaveChanges();
        }


        /// <summary>
        /// <see cref="IAccountsRepository.RemoveRoleWithId"/>
        /// </summary>
        public void RemoveRoleWithId(Account account, int roleId)
        {
            Role role = Context.Roles.FirstOrDefault(r => r.Id == roleId);
            if (account == null || role == null || !account.Roles.Contains(role))
                return;

            account.Roles.Remove(role);
            Context.SaveChanges();
        }

        /// <summary>
        /// <see cref="IAccountsRepository.GetAccountById"/>
        /// </summary>
        public Account GetAccountById(int id)
        {
            return Context.Accounts.FirstOrDefault(acc => acc.Id == id);
        }

        /// <summary>
        /// <see cref="IAccountsRepository.GetAccountId"/>
        /// </summary>
        public int? GetAccountId(string login)
        {
            try
            {
                string searchLogin = GetLowerTrimedLogin(login);
                return Context.Accounts.Where(acc => acc.UserLogin == searchLogin).Select(acc => acc.Id).First();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// <see cref="IAccountsRepository.GetAccountRolesNames"/>
        /// </summary>
        public IList<string> GetAccountRolesNames(int accountId)
        {
            Account account = GetAccountById(accountId);
            if (account == null)
                return new List<string>();
            return account.Roles.Select(role => role.DisplayName).ToList();
        }

        /// <summary>
        /// <see cref="IAccountsRepository.GetRoles"/>
        /// </summary>
        public IList<Role> GetRoles()
        {
            return Context.Roles.ToList();
        }

        /// <summary>
        /// <see cref="IAccountsRepository.GetRoleWithCode"/>
        /// </summary>
        /// <returns></returns>
        public Role GetRoleWithCode(string roleCode)
        {
            return Context.Roles.FirstOrDefault(role => role.Code.Equals(roleCode));
        }

        /// <summary>
        /// <see cref="IAccountsRepository.AddActivationCode"/>
        /// </summary>
        public void AddActivationCode(AccountActivationCode accountActivationCode)
        {
            if (accountActivationCode == null)
                return;

            IList<AccountActivationCode> oldCodes= Context.AccountActivationCodes.Where(a => a.Account.Id == accountActivationCode.Account.Id).ToList();
            foreach (AccountActivationCode activationCode in oldCodes)
            {
                Context.AccountActivationCodes.Remove(activationCode);
            }

            RemoveOldCodes();

            accountActivationCode.LastChangedDate = DateTime.Now;
            Context.AccountActivationCodes.Add(accountActivationCode);

            Context.SaveChanges();
        }

        /// <summary>
        /// <see cref="IAccountsRepository.GetIfActivationCodeLinkedToAccountExists"/>
        /// </summary>
        public bool GetIfActivationCodeLinkedToAccountExists(int accountId, string code)
        {
            if (string.IsNullOrEmpty(code))
                return false;

            AccountActivationCode accountActivationCode = Context.AccountActivationCodes.FirstOrDefault(c => c.Account.Id == accountId && c.Code.Trim() == code.Trim());

            if (accountActivationCode != null)
            {
                Context.AccountActivationCodes.Remove(accountActivationCode);
                Context.SaveChanges();
            }

            return accountActivationCode != null;
        }

        /// <summary>
        /// <see cref="IAccountsRepository.GetPageTileById"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public PageTile GetPageTileById(int id)
        {
            return Context.PageTiles.FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// <see cref="IAccountsRepository.AddEditPageTile"/>
        /// </summary>
        public void AddEditPageTile(PageTile pageTile)
        {
            if (pageTile.Id == 0)
            {
                Context.PageTiles.Add(pageTile);
            }
            else
            {
                Context.Entry(pageTile).State = EntityState.Modified;
            }

            Context.SaveChanges();
        }

        /// <summary>
        /// <see cref="IAccountsRepository.RemovePageTile"/>
        /// </summary>
        public void RemovePageTile(PageTile pageTile)
        {
            Context.PageTiles.Remove(pageTile);
            Context.SaveChanges();   
        }

        private void RemoveOldCodes()
        {
            IList<AccountActivationCode> codesToRemove = new List<AccountActivationCode>();
            foreach (AccountActivationCode accountActivationCode in Context.AccountActivationCodes)
            {
                if (accountActivationCode.LastChangedDate.AddHours(activationWaitTimeInHours) < DateTime.Now)
                    codesToRemove.Add(accountActivationCode);
            }

            foreach (AccountActivationCode accountActivationCode in codesToRemove)
            {
                Context.AccountActivationCodes.Remove(accountActivationCode);
            }
        }

        private string GetLowerTrimedLogin(string login)
        {
            if (login == null)
                return null;

            return login.Trim().ToLowerInvariant();
        }       
    }
}
