namespace BTTechnologies.MedScience.MVC.BTTClasses
{
    public class BTTAjaxInputGridModel
    {
        public string[] ColumnNames { get; set; }
        public int RowsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public string SortableColumnId { get; set; }
        public bool AscentSort { get; set; }
    }
}