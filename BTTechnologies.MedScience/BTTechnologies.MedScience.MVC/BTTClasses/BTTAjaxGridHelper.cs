using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace BTTechnologies.MedScience.MVC.BTTClasses
{
    public static class BTTAjaxGridHelper
    {
        public static BTTAjaxOutputGridModel GetOrderedGridData<TTable>(IOrderedQueryable<TTable> dbSet, BTTAjaxInputGridModel inputGridModel, IList<ChangeValueInformation> valuesToChange = null) where TTable : class
        {
            if (dbSet == null || inputGridModel == null || inputGridModel.ColumnNames == null || !inputGridModel.ColumnNames.Any())
                return null;

            IQueryable<TTable> tableObjects = dbSet.Skip((inputGridModel.CurrentPage - 1) * inputGridModel.RowsPerPage).Take(inputGridModel.RowsPerPage);
            IList<TTable> objectsList = tableObjects.ToList();

            BTTAjaxOutputGridModel outputGridModel = new BTTAjaxOutputGridModel(objectsList.Count()) { TotalRowsCount = dbSet.Count() };

            int index = 0;
            foreach (TTable table in objectsList)
            {
                outputGridModel.ResultValues[index] = GetItemsPropertyValues(table, inputGridModel.ColumnNames, valuesToChange);
                index++;
            }

            return outputGridModel;
        }

        public static BTTAjaxOutputGridModel GetGridData<TTable>(DbSet<TTable> dbSet, BTTAjaxInputGridModel inputGridModel, IList<ChangeValueInformation> valuesToChange = null) where TTable : class 
        {
            if (inputGridModel == null || inputGridModel.ColumnNames == null || !inputGridModel.ColumnNames.Any())
                return null;

            return GetOrderedGridData(dbSet.OrderByExpression(inputGridModel), inputGridModel, valuesToChange);
        }

        private static IOrderedQueryable<TTable> OrderByExpression<TTable>(this IQueryable<TTable> query, BTTAjaxInputGridModel inputGridModel)
        {
            PropertyInfo propertyInfo = string.IsNullOrEmpty(inputGridModel.SortableColumnId)
                                            ? GetTypeProperties<TTable>().First()
                                            : (typeof (TTable).GetProperty(inputGridModel.SortableColumnId) ??
                                               GetTypeProperties<TTable>().First());

            ParameterExpression[] typeParams = { Expression.Parameter(typeof(TTable), "") };
            
            return (IOrderedQueryable<TTable>)query.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    inputGridModel.AscentSort ? "OrderBy" : "OrderByDescending",
                    new[] { typeof(TTable), propertyInfo.PropertyType },
                    query.Expression,
                    Expression.Lambda(Expression.Property(typeParams[0], propertyInfo), typeParams))
            );
        }

        private static IEnumerable<PropertyInfo> GetTypeProperties<T>()
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => !p.GetGetMethod().IsVirtual).ToList();
        }

        private static string[] GetItemsPropertyValues<TTable>(TTable table, string[] columnNames, IList<ChangeValueInformation> valuesToChange)
        {
            int index = 0;
            string[] array = new string[columnNames.Count()];
            
            foreach (string columnName in columnNames)
            {
                PropertyInfo info = table.GetType().GetProperty(columnName);
                if (info == null || info.GetGetMethod().IsVirtual)
                {
                    array[index] = string.Empty;
                    index++;
                    continue;
                }

                object value = info.GetValue(table, null);
                array[index] = GetValue(value, valuesToChange == null ? new List<ChangeValueInformation>() : valuesToChange.Where(v => v.ColumnType == info.PropertyType));
                
                index++;
            }

            return array;
        }

        private static string GetValue(object value, IEnumerable<ChangeValueInformation> valuesToChange)
        {
            string newValue = value == null ? string.Empty: value.ToString();
            ChangeValueInformation info = valuesToChange == null ? null : valuesToChange.FirstOrDefault(v => v.CompareDelegate(newValue));
            return info == null || info.GetValueDelegate(value) == null ? newValue : info.GetValueDelegate(value);
        }

        public class ChangeValueInformation
        {
            private Func<object, bool> compareDelegate;
            private Func<object, string> getValueDelegate;

            public Type ColumnType { get; private set; } 
            public String ChangeFrom { get; private set; }
            public String ChangeTo { get; private set; }

            public Func<object, string> GetValueDelegate
            {
                get { return getValueDelegate ?? GetValue; }
                set { getValueDelegate = value; }
            }

            public Func<object, bool> CompareDelegate
            {
                get { return compareDelegate ?? CompareStrings; }
                set { compareDelegate = value; }
            }

            public ChangeValueInformation(Type columnType, string fromValue, string toValue)
            {
                ColumnType = columnType;
                ChangeFrom = fromValue;
                ChangeTo = toValue;
            }

            private bool CompareStrings(object objToCompare)
            {
                if (objToCompare == null)
                    return false;

                return ChangeFrom == objToCompare.ToString();
            }

            private string GetValue(object objToGetInstead)
            {
                return objToGetInstead == null ? null : ChangeTo;
            }
        }
    }
}