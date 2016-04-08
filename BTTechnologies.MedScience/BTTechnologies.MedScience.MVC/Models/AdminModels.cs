using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using BTTechnologies.MedScience.Domain.Entities;
using BTTechnologies.MedScience.MVC.App_LocalResources;

namespace BTTechnologies.MedScience.MVC.Models
{
    public class ManageUserDataModel : SelectedUserPermissions
    {
        [Display(ResourceType = typeof(MedSiteStrings), Name = "UserEmail")]
        public string UserLogin { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "Name")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "Surname")]
        public string Surname { get; set; }
        
        [Display(ResourceType = typeof(MedSiteStrings), Name = "Patronymic")]
        public string Patronymic { get; set; }
        
        [Display(ResourceType = typeof(MedSiteStrings), Name = "Phone")]
        public string Phone { get; set; }
        
        [Display(ResourceType = typeof(MedSiteStrings), Name = "RegistrationDate")]
        public DateTime? RegistrationDate { get; set; }
        
        [Display(ResourceType = typeof(MedSiteStrings), Name = "LastLogIn")]
        public DateTime? LastLogInDate { get; set; }
    }

    public class SelectedUserPermissions
    {
        public int Id { get; set; }
        public string RefreshUrl { get; set; }
        public string AddPermissionUrl { get; set; }
        public string DeletePermissionUrl { get; set; }
    }

    public class UserPermission
    {
        public string RoleCode;
        public string RoleDisplayName;
    }

    public class ManagePageTileModel : IOkErrorModel
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "WhereToShowContent")]
        [Required(ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "FieldShouldBeFilled")]
        public PageTypes PageType { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "TileType")]
        [Required(ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "FieldShouldBeFilled")]
        public TileTypes TileType { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "CSSStylesOfConteiner")]
        public string TileStyles { get; set; }

        [AllowHtml]
        [Display(ResourceType = typeof(MedSiteStrings), Name = "ConteinerContent")]
        public string Content { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "OrderOfConteiner")]
        [Required(ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "FieldShouldBeFilled")]
        public int ItemOrder { get; set; }

        public bool? OperationSucceed { get; set; }
    }
}