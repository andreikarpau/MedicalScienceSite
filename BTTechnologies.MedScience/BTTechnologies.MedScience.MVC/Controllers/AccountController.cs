using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using BTTechnologies.MedScience.Domain.Entities;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using BTTechnologies.MedScience.MVC.Infrastructure;
using BTTechnologies.MedScience.MVC.Infrastructure.Messages;
using BTTechnologies.MedScience.MVC.Infrastructure.Security;
using CaptchaMvc.HtmlHelpers;
using BTTechnologies.MedScience.MVC.Models;

namespace BTTechnologies.MedScience.MVC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountService accountService;
        private readonly IMembershipService membershipService;

        public AccountController(IAccountService newAccountService, IMembershipService newMembershipService)
        {
            accountService = newAccountService;
            membershipService = newMembershipService;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return View(new LoginModel {ReturnUrl = returnUrl});
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            bool captchaValid = this.IsCaptchaValid(EnumDescriptionHelper.GetEnumDescription(MedScienceErrors.RecaptchaIsIncorrect));

            if (captchaValid && ModelState.IsValid && membershipService.LogInUser(model.UserName, model.Password, model.RememberMe))
                return RedirectToLocal(returnUrl);

            if (captchaValid)
                ModelState.AddModelError(string.Empty, EnumDescriptionHelper.GetEnumDescription(MedScienceErrors.UserLoginOrPasswordIsIncorrect));
            else
                ModelState.AddModelError("captcha", EnumDescriptionHelper.GetEnumDescription(MedScienceErrors.RecaptchaIsIncorrect));

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (!this.IsCaptchaValid(EnumDescriptionHelper.GetEnumDescription(MedScienceErrors.RecaptchaIsIncorrect)))
                ModelState.AddModelError("captcha", EnumDescriptionHelper.GetEnumDescription(MedScienceErrors.RecaptchaIsIncorrect));

            if (ModelState.IsValid && model.Password == model.ConfirmPassword)
            {
                MedScienceErrors error = accountService.CreateNewAccount(model.UserName, model.Password);

                if (error == MedScienceErrors.NoError)
                {
                    membershipService.LogInUser(model.UserName, model.Password, true);
                    return RedirectToAction("ActivateUser");
                }

                ModelState.AddModelError(string.Empty, EnumDescriptionHelper.GetEnumDescription(error));
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ActivateUser()
        {
            Account user = membershipService.GetActiveAccount(HttpContext);
            membershipService.SendAccountActivationEmail(user.Id, GetUrl);

            return View(new ActivateAccountModel {Email = user.UserLogin});
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ActivateUser(ActivateAccountModel model)
        {
            int? newId = accountService.GetAccountId(model.Email);
            int id = newId == null ? 0: (int)newId;

            return RedirectToAction("VerifyActivationCode", new { id, value = model.VerificationCode });
        }

        [AllowAnonymous]
        public ActionResult VerifyActivationCode(int id, string value)
        {
            ActivateAccountModel model = new ActivateAccountModel();

            if (membershipService.VerifyActivationCode(id, value))
            {
                accountService.ActivateUser(id);

                model.Verified = true;
                return View("ActivateUser", model);
            }

            model.VerificationCode = string.Empty;
            membershipService.SendAccountActivationEmail(model.Email, GetUrl);
            ModelState.AddModelError(string.Empty, EnumDescriptionHelper.GetEnumDescription(MedScienceErrors.ActivationCodeIsNotRight));
            ModelState.Remove("VerificationCode");

            return View("ActivateUser", model);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult SendActivationEmail(string email)
        {
            if (!string.IsNullOrEmpty(email))
                membershipService.SendAccountActivationEmail(email, GetUrl);

            return new JsonResult();
        }

        [Permission]
        public ActionResult Manage()
        {
            ManageAccountModel model = accountService.GetUserAccountModel(HttpContext.User.Identity.Name);
            model.UserTmpEmail = HttpContext.User.Identity.Name;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission]
        public ActionResult Manage(ManageAccountModel model)
        {
            model.OperationSucceed = false;

            if (ModelState.IsValid)
            {
                if (accountService.SaveUserAccount(model, HttpContext.User.Identity.Name))
                {
                    model.OperationSucceed = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, EnumDescriptionHelper.GetEnumDescription(MedScienceErrors.UnknownError));
                }
            }

            model.UserTmpEmail = HttpContext.User.Identity.Name;
            return View(model);
        }

        [Permission]
        public ActionResult ChangeLogin()
        {
            ChangeLoginModel model = accountService.GetChangeLoginModel(HttpContext.User.Identity.Name);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission]
        public ActionResult ChangeLogin(ChangeLoginModel model)
        {
            if (ModelState.IsValid && model.UserLogin != null &&
                model.UserLogin.Trim() != HttpContext.User.Identity.Name.Trim())
            {
                MedScienceErrors error = accountService.ChangeAccountLogin(model.Id, model.UserLogin);

                if (error == MedScienceErrors.NoError)
                {
                    FormsAuthentication.SignOut();
                    Account account = accountService.GetAccountByUserLogin(model.UserLogin);
                    membershipService.LogInUser(account.UserLogin, account.UserPassword, true);
                    return RedirectToAction("ActivateUser");
                }

                ModelState.AddModelError("UserLogin", EnumDescriptionHelper.GetEnumDescription(error));
            }

            model.OperationSucceed = false;
            return View(model);
        }

        [Permission]
        public ActionResult ChangePassword()
        {
            ChangeUserPasswordModel model = accountService.GetCurrentUserPasswordModel(HttpContext.User.Identity.Name);
            return View(model);
        }

        [HttpPost]
        [Permission]
        public ActionResult ChangePassword(ChangeUserPasswordModel model)
        {
            if (ModelState.IsValid && model.Password == model.ConfirmPassword)
            {
                MedScienceErrors error = accountService.ChangeUserPassword(model.Id, model.OldPassword, model.Password);

                if (error == MedScienceErrors.NoError)
                {
                    return View(new ChangeUserPasswordModel { Id = model.Id, OperationSucceed = true });
                }

                ModelState.AddModelError(error == MedScienceErrors.IncorrectPassword ? "OldPassword" : string.Empty,
                                         EnumDescriptionHelper.GetEnumDescription(error));
            }

            model.OperationSucceed = false;
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult RestorePassword(int? id)
        {
            RestorePasswordModel model = id == null ? new RestorePasswordModel() : accountService.GetRestorePaswordModelById((int)id);            
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryTokenAttribute]
        public ActionResult RestorePassword(RestorePasswordModel model)
        {
            model.Success = false;

            if (!membershipService.VerifyVerificationCode(HttpContext, model.VerificationCode, model.UserLogin))
                ModelState.AddModelError(string.Empty, EnumDescriptionHelper.GetEnumDescription(MedScienceErrors.IncorrectVerificationCode));

            if (ModelState.IsValid)
            {
                MedScienceErrors error = accountService.ChangeUserPassword(model.UserLogin, model.Password);
                if (error == MedScienceErrors.NoError)
                    model.Success = true;
                else
                    ModelState.AddModelError(string.Empty, EnumDescriptionHelper.GetEnumDescription(error));
            }

            ModelState.Remove("VerificationCode");
            model.VerificationCode = null;           
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult GetRestoreEmail()
        {
            return View(new GetEmailModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetRestoreEmail(GetEmailModel model)
        {
            if (!this.IsCaptchaValid(EnumDescriptionHelper.GetEnumDescription(MedScienceErrors.RecaptchaIsIncorrect)))
                ModelState.AddModelError("captcha", EnumDescriptionHelper.GetEnumDescription(MedScienceErrors.RecaptchaIsIncorrect));

            if (ModelState.IsValid && model.UserLogin != null)
            {
                int? userId = accountService.GetAccountId(model.UserLogin.Trim());
                if (userId == null)
                {
                    ModelState.AddModelError("UserLogin", EnumDescriptionHelper.GetEnumDescription(MedScienceErrors.NoUserWithSuchEmail));
                }
                else
                {
                    membershipService.SaveRandomVerificationCodeToSessionAndSendEmail(HttpContext, model.UserLogin.Trim());
                    return RedirectToAction("RestorePassword", new { id = userId });
                }
            }

            return View(model);
        }


        #region Helpers
        private string GetUrl(int i, string s)
        {
            return Url.Action("VerifyActivationCode", "Account", new RouteValueDictionary(new { id = i, value = s }), Request.Url.Scheme, null);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}
