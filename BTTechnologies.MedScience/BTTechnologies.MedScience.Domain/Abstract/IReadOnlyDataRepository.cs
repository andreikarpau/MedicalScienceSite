using System.Collections.Generic;
using BTTechnologies.MedScience.Domain.Entities;

namespace BTTechnologies.MedScience.Domain.Abstract
{
    /// <summary>
    /// Reporsitory which alows to read data from database
    /// </summary>
    public interface IReadOnlyDataRepository
    {
        /// <summary>
        /// Get list of tiles by page type
        /// </summary>
        /// <param name="pageType"></param>
        /// <returns></returns>
        IEnumerable<PageTile> GetPageTilesByPageType(PageTypes pageType);
    }
}
