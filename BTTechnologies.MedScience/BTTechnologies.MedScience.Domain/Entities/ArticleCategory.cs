using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BTTechnologies.MedScience.Domain.Entities
{
    /// <summary>
    /// Article category
    /// </summary>
    public class ArticleCategory
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ArticleCategory()
        {
// ReSharper disable DoNotCallOverridableMethodsInConstructor
            Articles = new HashSet<Article>();
// ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        /// <summary>
        /// Category Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Category display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Category short description
        /// </summary>
        public string CategoryDescription { get; set; }
        
        /// <summary>
        /// Category article
        /// </summary>
        public virtual ICollection<Article> Articles { get; set; }
    }
}
