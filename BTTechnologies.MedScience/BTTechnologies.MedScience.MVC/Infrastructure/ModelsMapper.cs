using System;
using System.Linq;
using System.Reflection;
using BTTechnologies.MedScience.MVC.Helpers;

namespace BTTechnologies.MedScience.MVC.Infrastructure
{
    /// <summary>
    /// This class mapps properties with equal names
    /// </summary>
    public class ModelsMapper
    {
        public static bool CreateNewModelUsingMapping<TMapTo, TMapFrom>(TMapTo mapToModel, TMapFrom mapFromModel) where TMapTo : class where TMapFrom : class
        {
            if (mapToModel == null || mapFromModel == null)
                return false;

            try
            {
                PropertyInfo[] mapToProperties = typeof(TMapTo).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                PropertyInfo[] mapFromProperties = typeof(TMapFrom).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (PropertyInfo mapFromProperty in mapFromProperties.Where(mapFromProperty => mapToProperties.Any(prop => prop.Name == mapFromProperty.Name && prop.PropertyType == mapFromProperty.PropertyType)))
                {
                    object value = mapFromModel.GetType().GetProperty(mapFromProperty.Name, mapFromProperty.PropertyType).GetValue(mapFromModel, null);
                    mapToModel.GetType().GetProperty(mapFromProperty.Name, mapFromProperty.PropertyType).SetValue(mapToModel, value, null);
                }
            }
            catch (Exception e)
            {
                ExceptionsLogger.LogException(e); 
                return false;
            }

            return true;
        }
    }
}