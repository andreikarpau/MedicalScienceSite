using System;
using System.ComponentModel.DataAnnotations;

namespace BTTechnologies.MedScience.Domain.Entities
{
    /// <summary>
    /// Article statistical data recoed
    /// </summary>
    public class ArticlesFullDataRecord
    {
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
        /// Article authors names
        /// </summary>
        public string AuthorsNames { get; set; }

        /// <summary>
        /// Articles attachments count
        /// </summary>
        public int AttachmentsCount { get; set; }

        /// <summary>
        /// Categories names
        /// </summary>
        public string Categories { get; set; }

        /// <summary>
        /// Shows if article is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Date and time when article was last changed
        /// </summary>
        public DateTime LastChangedDate { get; set; }
    }
}
