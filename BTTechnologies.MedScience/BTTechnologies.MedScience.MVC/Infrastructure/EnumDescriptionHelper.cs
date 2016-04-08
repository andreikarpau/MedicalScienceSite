using System;
using System.Reflection;

namespace BTTechnologies.MedScience.MVC.Infrastructure
{
    public static class EnumDescriptionHelper
    {
        public static string GetEnumDescription(Enum value)
        {
            Type type = value.GetType();
            FieldInfo fi = type.GetField(value.ToString());
            object[] attrs = fi.GetCustomAttributes(typeof(EnumDescriptionAttribute), false);

            if (attrs.Length <= 0)
                return string.Empty;

            EnumDescriptionAttribute attribute = attrs[0] as EnumDescriptionAttribute;
            if (attribute == null)
                return string.Empty;

            if (!string.IsNullOrEmpty(attribute.Value))
                return attribute.Value;

            if (attribute.ResourcesType != null && !string.IsNullOrEmpty(attribute.ResourcesName))
            {
                object stringValue = attribute.ResourcesType.GetProperty(attribute.ResourcesName).GetValue(null, null);
                return stringValue == null ? string.Empty : stringValue.ToString();
            }

            return string.Empty;
        }
    }
}