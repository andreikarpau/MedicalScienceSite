using System.Collections.Generic;

namespace BTTechnologies.MedScience.MVC.BTTClasses
{
    public class BTTAjaxGridModel
    {
        private readonly IList<BTTGridColumn> columnsCollection = new List<BTTGridColumn>();

        public string TableName { get; set; }
        public string DataActionUrl { get; set; }
        public string EditRowUrl { get; set; }
        public string DeleteRowUrl { get; set; }
        public string DeleteConfirmationText { get; set; }
        public string DeleteTooltipText { get; set; }
        public string EditTooltipText { get; set; }

        public bool AddEditColumn { get; set; }
        public bool AddDeleteColumn { get; set; }

        public IList<BTTGridColumn> Columns
        {
            get { return columnsCollection; }
        }

        public IList<int> RowsPerPage
        {
            get;
            set;
        }

        public BTTAjaxGridModel()
        {
            TableName = "BTTAjaxGrid";
            RowsPerPage = new List<int> { 10, 20, 50 };
            AddEditColumn = false;
            AddDeleteColumn = false;
            DeleteTooltipText = "Delete";
            EditTooltipText = "Edit";
        }

        public BTTAjaxGridModel(string tableName, string dataActionUrl)
            : this()
        {
            TableName = tableName;
            DataActionUrl = dataActionUrl;
        }
    }

    public class BTTGridColumn
    {
        public string ColumnIdentifier { get; set; }
        public string DisplayName { get; set; }
        public bool IsSortable { get; set; }
        public bool IsKey { get; set; }
        public bool IsHidden { get; set; }
    }
}