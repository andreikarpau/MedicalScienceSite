using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BTTechnologies.MedScience.Domain.Entities
{
    /// <summary>
    /// Articles attachments
    /// </summary>
    public class DocAttachment
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DocAttachment()
        {
            Articles = new HashSet<Article>();
        }

        /// <summary>
        /// Identifier
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Attachment display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Attachments url
        /// </summary>
        public string AttachmentURL { get; set; }

        /// <summary>
        /// Attachment Type
        /// </summary>
        public string AttachmentType { get; set; }

        /// <summary>
        /// Attachment download options
        /// </summary>
        public string DownloadOptions { get; set; }

        /// <summary>
        /// Articles
        /// </summary>
        public virtual ICollection<Article> Articles { get; set; }
    }
}
