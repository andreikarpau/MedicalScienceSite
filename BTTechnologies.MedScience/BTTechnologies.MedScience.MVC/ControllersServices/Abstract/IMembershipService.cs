using System;
using System.Collections.Generic;
using System.Web;
using BTTechnologies.MedScience.Domain.Entities;
using BTTechnologies.MedScience.MVC.Infrastructure.Security;

namespace BTTechnologies.MedScience.MVC.ControllersServices.Abstract
{
    public interface IMembershipService
    {
        IList<string> GetAdminRolesCodes();
        bool UserIsAdmin(int id, bool checkActivation = true);
        bool UserIsAdmin(string userLogin, bool checkActivation = true);
        bool LogInUser(string userLogin, string password, bool rememberUser);
        Account GetActiveAccount(HttpContextBase httpContext);
        Account GetActiveAccount(string login);
        void SaveRandomVerificationCodeToSessionAndSendEmail(HttpContextBase httpContext, string email);
        bool VerifyVerificationCode(HttpContextBase httpContext, string code, string email);
        bool CurrentUserIsActivated(string userName);
        bool UserHasPrivilege(string userName, params Privilege[] privileges);
        bool UserHasPrivilege(HttpContextBase httpContext, params Privilege[] privileges);
        void SendAccountActivationEmail(int accountId, Func<int, string, string> getUrl);
        void SendAccountActivationEmail(string login, Func<int, string, string> getUrl);
        bool VerifyActivationCode(int id, string code);
    }
}