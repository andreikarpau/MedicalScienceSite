using BTTechnologies.MedScience.MVC.App_LocalResources;

namespace BTTechnologies.MedScience.MVC.Infrastructure.Messages
{
    public enum MedScienceErrors
    {
        [EnumDescription("")]
        NoError,

        [EnumDescription(typeof(MedSiteStrings), "UnknownErrorOccured")]
        UnknownError,
        
        [EnumDescription(typeof(MedSiteStrings), "WrongLoginOrPassword")]
        UserLoginOrPasswordIsIncorrect,
        
        [EnumDescription(typeof(MedSiteStrings), "RecaptchaError")]
        RecaptchaIsIncorrect,

        [EnumDescription(typeof(MedSiteStrings), "AccountAlreadyExistsErrorWithHelp")]
        AccountWithSameLoginAlreadyExistsTryToBackupYouPassword,

        [EnumDescription(typeof(MedSiteStrings), "AccountAlreadyExistsError")]
        AccountWithSameLoginAlreadyExists,

        [EnumDescription(typeof(MedSiteStrings), "WrongActivationCodeError")]
        ActivationCodeIsNotRight,

        [EnumDescription(typeof(MedSiteStrings), "LoginNotFoundError")]
        NoUserWithSuchEmail,

        [EnumDescription(typeof(MedSiteStrings), "IncorrectVerificationCodeError")]
        IncorrectVerificationCode,

        [EnumDescription(typeof(MedSiteStrings), "IncorrectPasswordError")]
        IncorrectPassword,

        [EnumDescription(typeof(MedSiteStrings), "AdminCannotDeleteHimSelfError")]
        AdminCannotDeleteHimself,

        [EnumDescription(typeof(MedSiteStrings), "OnlySuperUserCanDeleteOtherAdminsError")]
        OnlySuperAdminCanDeleteOtherAdmins,

        [EnumDescription(typeof(MedSiteStrings), "AdminCannotChangeHisRoles")]
        AdminCannotChangeHisRoles,

        [EnumDescription(typeof(MedSiteStrings), "WrongEmailError")]
        WrongEmail,

        [EnumDescription(typeof(MedSiteStrings), "NonAdminCannotManageNotLinkedAuthor")]
        NonAdminCannotManageNotLinkedAuthor,

        [EnumDescription(typeof(MedSiteStrings), "CategoryExistsError")]
        CategoryAlreadyExistsError,

        [EnumDescription(typeof(MedSiteStrings), "ArticleTitleCannotBeEmptyError")]
        ArticleNameCannotBeEmpty,

        [EnumDescription(typeof(MedSiteStrings), "ArticleTitleCannotBeEmptyError")]
        ErrorsWhenSavingFiles,

        [EnumDescription(typeof(MedSiteStrings), "AttachmentFilesSavingError")]
        ArticleWithSuchNameAlreadyExistsError,

        [EnumDescription(typeof(MedSiteStrings), "AccessError")]
        NoAccessError
    }
}