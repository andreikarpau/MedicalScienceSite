using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using BTTechnologies.MedScience.MVC.App_LocalResources;
using BTTechnologies.MedScience.MVC.Infrastructure.Security;

namespace BTTechnologies.MedScience.MVC.Models
{
    public class LoginModel
    {
        public LoginModel()
        {
            RememberMe = true;
        }

        [Required(ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "FieldShouldBeFilled")]
        [Display(ResourceType = typeof(MedSiteStrings), Name = "YourEmail")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "FieldShouldBeFilled")]
        [Display(ResourceType = typeof(MedSiteStrings), Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "NeedSaveQuestion")]
        public bool RememberMe { get; set; }

        [HiddenInput]
        public string ReturnUrl { get; set; }
    }

    public class RegisterModel
    {
        [DataType(DataType.EmailAddress)]
        [EmailValidation]
        [Required(ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "FieldShouldBeFilled")]
        [Display(ResourceType = typeof(MedSiteStrings), Name = "YourEmail")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "FieldShouldBeFilled")]
        [StringLength(100, ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "PasswordLengthError", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(MedSiteStrings), Name = "YourPassword")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "FieldShouldBeFilled")]
        [Display(ResourceType = typeof(MedSiteStrings), Name = "PasswordConfirmation")]
        [Compare("Password", ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "WrongPasswordConfirmation")]
        public string ConfirmPassword { get; set; }

    }

    public class ActivateAccountModel
    {
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "FieldShouldBeFilled")]
        [Display(ResourceType = typeof(MedSiteStrings), Name = "AccountActivationCode")]
        public string VerificationCode { get; set; }
        public bool Verified { get; set; }
    }

    public class ManageAccountModel: IOkErrorModel
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "UserEmail")]
        public string UserTmpEmail { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "Name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "Surname")]
        public string Surname { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "Patronymic")]
        public string Patronymic { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "FullPhoneNumberWithExample")]
        public string Phone { get; set; }

        public int? AuthorId { get; set; }
    
        public bool? OperationSucceed { get; set; }
    }

    public class ChangeLoginModel : IOkErrorModel
    {
        public int Id { get; set; }
        [Display(ResourceType = typeof(MedSiteStrings), Name = "NewEmail")]
        [Required(ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "FieldShouldBeFilled")]
        [DataType(DataType.EmailAddress)]
        [EmailValidation]
        public string UserLogin { get; set; }

        public bool? OperationSucceed { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required(ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "FieldShouldBeFilled")]
        [StringLength(100, ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "PasswordLengthError", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(MedSiteStrings), Name = "YourNewPassword")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "FieldShouldBeFilled")]
        [Display(ResourceType = typeof(MedSiteStrings), Name = "PasswordConfirmation")]
        [Compare("Password", ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "WrongPasswordConfirmation")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangeUserPasswordModel : ChangePasswordModel, IOkErrorModel
    {
        public int Id { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "FieldShouldBeFilled")]
        [Display(ResourceType = typeof(MedSiteStrings), Name = "OldPassword")]
        public string OldPassword { get; set; }

        public bool? OperationSucceed { get; set; }
    }

    public class GetEmailModel
    {
        [Display(ResourceType = typeof(MedSiteStrings), Name = "YourEmail")]
        [Required(ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "FieldShouldBeFilled")]
        [DataType(DataType.EmailAddress)]
        [EmailValidation]
        public string UserLogin { get; set; }
    }

    public class RestorePasswordModel: ChangePasswordModel
    {
        public string UserLogin { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "FieldShouldBeFilled")]
        [Display(ResourceType = typeof(MedSiteStrings), Name = "RestorationAccountCode")]
        public string VerificationCode { get; set; }
        
        public bool? Success { get; set; }
    }
}
