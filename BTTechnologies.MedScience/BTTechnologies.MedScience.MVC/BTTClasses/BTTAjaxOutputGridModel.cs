namespace BTTechnologies.MedScience.MVC.BTTClasses
{
    public class BTTAjaxOutputGridModel
    {
        public object[] ResultValues { get; set; }
        public int TotalRowsCount { get; set; }

        public BTTAjaxOutputGridModel()
        {
            ResultValues = new object[0];
        }

        public BTTAjaxOutputGridModel(int valuesCount)
        {
            ResultValues = new object[valuesCount];
        }
    }
}