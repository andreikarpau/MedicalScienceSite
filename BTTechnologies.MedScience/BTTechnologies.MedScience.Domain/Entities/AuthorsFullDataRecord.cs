using System;

namespace BTTechnologies.MedScience.Domain.Entities
{
    /// <summary>
    /// Authors statistical data entity
    /// </summary>
    public class AuthorsFullDataRecord
    {
        /// <summary>
        /// Identifier
        /// </summary>
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
        public Boolean AuthorStatus { get; set; }

        /// <summary>
        /// Linked account login
        /// </summary>
        public string LinkedLogin { get; set; }

        /// <summary>
        /// Count of articles of the author  
        /// </summary>
        public int ArticlesCount { get; set; }
    }
}
