using System.ComponentModel.DataAnnotations.Schema;

namespace BTTechnologies.MedScience.Domain.Entities
{
    /// <summary>
    /// Types of the page where page tiles are shown
    /// </summary>
    public enum PageTypes
    {
        /// <summary>
        /// Main page
        /// </summary>
        MainPage = 1,

        /// <summary>
        /// About page
        /// </summary>
        AboutPage = 2
    }

    /// <summary>
    /// Types of the tile
    /// </summary>
    public enum TileTypes
    {
        /// <summary>
        /// Default tile
        /// </summary>
        Default = 1,

        /// <summary>
        /// New on site tile
        /// </summary>
        NewOnSite = 2
    }

    /// <summary>
    /// Page tile
    /// </summary>
    public class PageTile
    {
        /// <summary>
        /// Unique Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Type of menu
        /// </summary>
        [NotMapped]
        public PageTypes PageType { get; set; }

        /// <summary>
        /// Id of type of menu
        /// </summary>
        [Column("PageType")]
        public int PageTypeId
        {
            get { return (int)PageType; }
            set { PageType = (PageTypes)value; }
        }
        
        /// <summary>
        /// Type of tile
        /// </summary>
        [NotMapped]
        public TileTypes TileType { get; set; }

        /// <summary>
        /// Id of type of tile
        /// </summary>
        [Column("TileType")]
        public int TileTypeId
        {
            get { return (int)TileType; }
            set { TileType = (TileTypes)value; }
        }
        
        /// <summary>
        /// Styles of this tile
        /// </summary>
        public string TileStyles { get; set; }

        /// <summary>
        /// Tile content (Html possible)
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Item order
        /// </summary>
        public int ItemOrder { get; set; }
    }
}
