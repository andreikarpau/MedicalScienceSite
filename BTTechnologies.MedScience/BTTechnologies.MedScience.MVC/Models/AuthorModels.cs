using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BTTechnologies.MedScience.MVC.App_LocalResources;
using BTTechnologies.MedScience.MVC.Infrastructure.Security;

namespace BTTechnologies.MedScience.MVC.Models
{
    public class AddEditAuthorModel : IOkErrorModel
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "Name")]
        [Required(ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "FieldShouldBeFilled")]
        public string Name { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "Surname")]
        [Required(ErrorMessageResourceType = typeof(MedSiteStrings), ErrorMessageResourceName = "FieldShouldBeFilled")]
        public string Surname { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "Patronymic")]
        public string Patronymic { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "DegreeAndOtherInfo")]
        public string Degree { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailValidation]
        public string EMail { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "FullPhoneNumberWithExample")]
        public string Phone { get; set; }       
       
        [Display(ResourceType = typeof(MedSiteStrings), Name = "AuthorStatusProved")]
        public bool AuthorStatus { get; set; }

        [Display(ResourceType = typeof(MedSiteStrings), Name = "AuthorsAccount")]
        public string AccountName { get; set; }

        public int? AccountId { get; set; }
        public bool? OperationSucceed { get; set; }

        public IDictionary<int, string> AccountIdsAndNames { get; set; }   
    }

    public class QuickAddAuthorModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Degree { get; set; }
    }
}