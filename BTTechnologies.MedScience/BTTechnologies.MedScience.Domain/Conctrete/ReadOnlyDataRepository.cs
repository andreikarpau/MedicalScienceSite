using System.Collections.Generic;
using System.Linq;
using BTTechnologies.MedScience.Domain.Abstract;
using BTTechnologies.MedScience.Domain.Entities;

namespace BTTechnologies.MedScience.Domain.Conctrete
{
    /// <summary>
    /// <see cref="IReadOnlyDataRepository"/>
    /// </summary>
    public class ReadOnlyDataRepository : BaseRepository, IReadOnlyDataRepository
    {
        /// <summary>
        /// <see cref="IReadOnlyDataRepository.GetPageTilesByPageType"/>
        /// </summary>
        /// <param name="pageType"></param>
        /// <returns></returns>
        public IEnumerable<PageTile> GetPageTilesByPageType(PageTypes pageType)
        {
            return Context.PageTiles.Where(t => t.PageTypeId == (int)pageType);
        }
    }
}
