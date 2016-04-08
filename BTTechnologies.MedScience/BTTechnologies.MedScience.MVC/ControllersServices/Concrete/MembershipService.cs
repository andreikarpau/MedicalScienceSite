using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using BTTechnologies.MedScience.Domain.Entities;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using BTTechnologies.MedScience.MVC.Helpers;
using BTTechnologies.MedScience.MVC.Infrastructure.Security;

namespace BTTechnologies.MedScience.MVC.ControllersServices.Concrete
{
    public class MembershipService : IMembershipService
    {
        private readonly IAccountService accountService; 
        private const string activationCodeValName = "ActivationCode";
        private const string activationCodeEmail = "ActivationCodeEmail";

        public MembershipService(IAccountService newAccountService)
        {
            accountService = newAccountService;
        }

        public IList<string> GetAdminRolesCodes()
        {
            return new List<string> {Role.ADMIN_ROLE_CODE, Role.SUPER_ADMIN_ROLE_CODE};
        }

        public bool UserIsAdmin(int id, bool checkActivation = true)
        {
            return UserIsAdmin(accountService.GetUserLoginById(id), checkActivation);
        }

        public bool UserIsAdmin(string userLogin, bool checkActivation = true)
        {
            if (checkActivation && !CurrentUserIsActivated(userLogin))
            {
                return false;
            }

            return UserIsInRole(userLogin, GetAdminRolesCodes().ToArray());
        }

        public bool LogInUser(string userLogin, string password, bool rememberUser)
        {
            if (string.IsNullOrEmpty(userLogin)||string.IsNullOrEmpty(password))
                return false;

            Account account = accountService.GetAccountByUserLogin(userLogin);

            if (account == null)
                return false;

            if (account.UserPassword.Equals(password))
            {
                FormsAuthentication.SetAuthCookie(userLogin, rememberUser);
                accountService.UpdateLastLogInDate(account);
                return true;
            }

            return false;
        }

        public Account GetActiveAccount(HttpContextBase httpContext)
        {
            return GetActiveAccount(httpContext.User.Identity.Name);
        }

        public Account GetActiveAccount(string login)
        {
            return accountService.GetAccountByUserLogin(login);
        }

        public void SendAccountActivationEmail(string login, Func<int, string, string> getUrl)
        {
            Account account = accountService.GetAccountByUserLogin(login);
            if (account == null)
                return;

            Random random = new Random();
            string code = random.Next(10000, 99999).ToString(CultureInfo.InvariantCulture);
            accountService.AddAccountActivationCode(account, code);
            string activationUrl = getUrl(account.Id, code);

            EmailHelper.SendActivationCodeEmail(login, code, activationUrl);
        }

        public void SendAccountActivationEmail(int accountId, Func<int, string, string> getUrl)
        {
            SendAccountActivationEmail(accountService.GetUserLoginById(accountId), getUrl);
        }

        public bool VerifyActivationCode(int id, string code)
        {
            string login = accountService.GetUserLoginById(id);
            if (CurrentUserIsActivated(login))
            {
                return true;
            }
            
            return accountService.VerifyActivationCode(id, code);
        }

        public void SaveRandomVerificationCodeToSessionAndSendEmail(HttpContextBase httpContext, string email)
        {
            Random random = new Random();
            httpContext.Session[activationCodeEmail] = email == null? null: email.Trim();
            httpContext.Session[activationCodeValName] = random.Next(10000, 99999).ToString(CultureInfo.InvariantCulture);
            EmailHelper.SendRestoreCodeEmail(email, httpContext.Session[activationCodeValName].ToString());
        }

        public bool VerifyVerificationCode(HttpContextBase httpContext, string code, string email)
        {
            string sessionEmail = httpContext.Session[activationCodeEmail] == null? null: httpContext.Session[activationCodeEmail].ToString();
            string activationCode = httpContext.Session[activationCodeValName] == null ? string.Empty: httpContext.Session[activationCodeValName].ToString();
            
            httpContext.Session[activationCodeEmail] = null;
            httpContext.Session[activationCodeValName] = null;

            return VerifyActivatioCode(code, activationCode, email, sessionEmail);
        }

        public bool CurrentUserIsActivated(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return false;

            IList<string> roleCodes = GetRolesCodesByUserLogin(userName);

            if (roleCodes == null)
                return false;

            return !roleCodes.Contains(Role.NOT_ACTIVATED_USER_ROLE_CODE);
        }

        public bool UserHasPrivilege(string userName, params Privilege[] privileges)
        {
            IList<Privilege> userPrivileges = RolesPrivileges.GetPrivileges(GetRolesCodesByUserLogin(userName));
            return privileges.Any(userPrivileges.Contains);
        }

        public bool UserHasPrivilege(HttpContextBase httpContext, params Privilege[] privileges)
        {
            if (!httpContext.Request.IsAuthenticated)
                return false;

            return UserHasPrivilege(httpContext.User.Identity.Name, privileges);
        }

        private bool VerifyActivatioCode(string code, string activationCode, string email, string sessionEmail)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(activationCode) || sessionEmail == null || email == null || sessionEmail.Trim() != email.Trim())
                return false;

            return code.Trim() == activationCode.Trim();
        }

        private bool UserIsInRole(string login, params string[] permissionCodes)
        {
            IList<string> userRoles = GetRolesCodesByUserLogin(login);
            foreach (string userRole in permissionCodes)
            {
                if (userRoles.Contains(userRole))
                    return true;
            }
            return false;
        }

        private IList<string> GetRolesCodesByUserLogin(string userLogin)
        {
            return accountService.GetRolesCodesByUserLogin(userLogin);
        }
    }
}