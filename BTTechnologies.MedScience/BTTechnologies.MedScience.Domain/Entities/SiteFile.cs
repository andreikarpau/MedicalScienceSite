using System.ComponentModel.DataAnnotations;

namespace BTTechnologies.MedScience.Domain.Entities
{
    /// <summary>
    /// File uploaded to site
    /// </summary>
    public class SiteFile
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// File display name 
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Url of the file
        /// </summary>
        public string FileUrl { get; set; }
    }
}
