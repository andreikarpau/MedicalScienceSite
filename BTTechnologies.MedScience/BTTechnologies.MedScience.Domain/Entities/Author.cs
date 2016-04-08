using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BTTechnologies.MedScience.Domain.Entities
{

    /// <summary>
    /// Document author
    /// </summary>
    public class Author
    {   
        /// <summary>
        /// Constructor
        /// </summary>
        public Author()
        {
            Articles = new HashSet<Article>();
        }

        /// <summary>
        /// Identifier
        /// </summary>
        [Key]
        public int Id { get; set; }

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
        /// Scientific degree and other authors information 
        /// </summary>
        public string Degree { get; set; }
        
        /// <summary>
        /// Phone (for administrators only) 
        /// </summary>
        public string Phone { get; set; }
        
        /// <summary>
        /// Email (for administrators only) 
        /// </summary>
        public string EMail { get; set; }   
     
        /// <summary>
        /// Link to author photo 
        /// </summary>
        public string PhotoLink { get; set; }

        /// <summary>
        /// Author is proved by administrator
        /// </summary>
        public bool AuthorStatus { get; set; }

        /// <summary>
        /// Author linked account id
        /// </summary>
        public int? AccountId { get; set; }

        /// <summary>
        /// Author linked account
        /// </summary>
        public virtual Account Account { get; set; }

        /// <summary>
        /// Author documents
        /// </summary>
        public virtual ICollection<Article> Articles { get; set; }
    }
}
