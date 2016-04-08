using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BTTechnologies.MedScience.Domain.Entities
{
    /// <summary>
    /// User Role
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Admin role code
        /// </summary>
        public const string SUPER_ADMIN_ROLE_CODE = "SuperAdminRole";
        /// <summary>
        /// Admin role code
        /// </summary>
        public const string ADMIN_ROLE_CODE = "AdminRole";
        /// <summary>
        /// User role code
        /// </summary>
        public const string USER_ROLE_CODE = "UserRole"; 
        /// <summary>
        /// Author role code
        /// </summary>
        public const string AUTHOR_ROLE_CODE = "AuthorRole";  
        /// <summary>
        /// Not activated user role code
        /// </summary>
        public const string NOT_ACTIVATED_USER_ROLE_CODE = "NotActivatedUserRole";

        /// <summary>
        /// Constructor
        /// </summary>
        public Role()
        {
            Accounts = new HashSet<Account>();
        }

        /// <summary>
        /// Identifier
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Role display name 
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Role code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Role accounts
        /// </summary>
        public virtual ICollection<Account> Accounts { get; set; }
    }
}
