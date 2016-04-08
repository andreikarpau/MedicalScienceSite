using System;
using System.ComponentModel.DataAnnotations;

namespace BTTechnologies.MedScience.Domain.Entities
{
    /// <summary>
    /// Article log entry
    /// </summary>
    public class ArticleChangesLog
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ArticleChangesLog()
        {
            LogDateTime = DateTime.Now;
        }

        /// <summary>
        /// Category Id
        /// </summary>
        [Key]
        public Int64 Id { get; set; }

        /// <summary>
        /// Item display name
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Item Id
        /// </summary>
        public int? ItemId { get; set; }

        /// <summary>
        /// Account that changed the item
        /// </summary>
        public int? AccountId { get; set; }

        /// <summary>
        /// Login if the account which changed the item
        /// </summary>
        public string LoginWhoChanged { get; set; }

        /// <summary>
        /// DB User who changed the item 
        /// </summary>
        public string DBUser { get; set; }

        /// <summary>
        /// DB User who changed the item 
        /// </summary>
        public string ChangesInformation { get; set; }

        /// <summary>
        /// Log date time 
        /// </summary>
        public DateTime LogDateTime { get; set; }
    }
}
