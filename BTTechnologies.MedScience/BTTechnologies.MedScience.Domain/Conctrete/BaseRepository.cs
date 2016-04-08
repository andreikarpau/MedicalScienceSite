using System.Linq;
using BTTechnologies.MedScience.Domain.Context;
using BTTechnologies.MedScience.Domain.Entities;

namespace BTTechnologies.MedScience.Domain.Conctrete
{
    /// <summary>
    /// Base repository
    /// </summary>
    public abstract class BaseRepository
    {
        private readonly MedScienceDBContext context = new MedScienceDBContext();

        /// <summary>
        /// Repository context
        /// </summary>
        public MedScienceDBContext Context
        {
            get { return context; }
        }

        /// <summary>
        /// Get article by id
        /// </summary>
        public Article GetArticleById(int id)
        {
            return Context.Articles.FirstOrDefault(a => a.Id == id);
        }

        /// <summary>
        ///Get article by name
        /// </summary>
        public Article GetArtilceByName(string displayName)
        {
            return Context.Articles.FirstOrDefault(a => a.DisplayName == displayName);
        }

        /// <summary>
        /// Get author by id
        /// </summary>
        public Author GetAuthorById(int id)
        {
            return Context.Authors.FirstOrDefault(a => a.Id == id);
        }

        /// <summary>
        /// Get category by id
        /// </summary>
        public ArticleCategory GetCategoryById(int id)
        {
            return Context.ArticleCategories.FirstOrDefault(c => c.Id == id);
        }
    }
}
