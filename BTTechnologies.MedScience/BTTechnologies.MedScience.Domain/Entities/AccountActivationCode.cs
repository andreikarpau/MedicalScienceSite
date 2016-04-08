using System;
using System.ComponentModel.DataAnnotations;

namespace BTTechnologies.MedScience.Domain.Entities
{
    /// <summary>
    /// Account activation code
    /// </summary>
    public class AccountActivationCode
    {
        /// <summary>
        /// Code entity Id
        /// </summary>
        [Key]
        public Int64 Id { get; set; } 
        
        /// <summary>
        /// Activation code
        /// </summary>
        public string Code { get; set; }
  
        /// <summary>
        /// Date when activation code was added
        /// </summary>
        public DateTime LastChangedDate { get; set; }  
        
        /// <summary>
        /// Linked Account
        /// </summary>
        public virtual Account Account { get; set; }
    }
}
