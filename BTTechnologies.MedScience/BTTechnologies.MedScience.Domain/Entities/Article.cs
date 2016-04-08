using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BTTechnologies.MedScience.Domain.Entities
{
    /// <summary>
    /// Article
    /// </summary>
    public class Article
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Article()
        {
// ReSharper disable DoNotCallOverridableMethodsInConstructor
            Authors = new HashSet<Author>();
            Attachments = new HashSet<DocAttachment>();
            Categories = new HashSet<ArticleCategory>();
// ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        /// <summary>
        /// Identifier
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Document display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Document description
        /// </summary>
        public string DocumentDescription { get; set; }

        /// <summary>
        /// Document content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Shows if article is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Date and time when article was last changed
        /// </summary>
        public DateTime LastChangedDate { get; set; }

        /// <summary>
        /// Date and time when article was created
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Document authors
        /// </summary>
        public virtual ICollection<Author> Authors { get; set; }

        /// <summary>
        /// Document attachments
        /// </summary>
        public virtual ICollection<DocAttachment> Attachments { get; set; }

        /// <summary>
        /// Document categories
        /// </summary>
        public virtual ICollection<ArticleCategory> Categories { get; set; }
    }
}
