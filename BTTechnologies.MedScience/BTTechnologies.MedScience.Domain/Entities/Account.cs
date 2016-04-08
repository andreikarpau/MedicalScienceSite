using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BTTechnologies.MedScience.Domain.Entities
{
    /// <summary>
    /// User account
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Account()
        {
            Roles = new HashSet<Role>();
        }
        
        /// <summary>
        /// Identifier
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Login of the users. Probably it will be user email.
        /// </summary>
        public string UserLogin { get; set; }
        /// <summary>
        /// User name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// User surname
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// Fathers name
        /// </summary>
        public string Patronymic { get; set; }
        /// <summary>
        /// User phone
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// User password
        /// </summary>
        public string UserPassword { get; set; }
        /// <summary>
        /// Date of user registration
        /// </summary>
        public DateTime? RegistrationDate { get; set; }
        /// <summary>
        /// Date of last entering the system
        /// </summary>
        public DateTime? LastLogInDate { get; set; }

        /// <summary>
        /// Account roles
        /// </summary>
        public virtual ICollection<Role> Roles { get; set; }

        /// <summary>
        /// Linked authors
        /// </summary>
        public virtual ICollection<Author> Authors { get; set; }
    }
}
