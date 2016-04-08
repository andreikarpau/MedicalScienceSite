using System.Collections.Generic;
using BTTechnologies.MedScience.Domain.Entities;

namespace BTTechnologies.MedScience.MVC.Models
{
    public class TilesPageModel
    {
        public IEnumerable<PageTile> PageTiles { get; set; } 
    }
}