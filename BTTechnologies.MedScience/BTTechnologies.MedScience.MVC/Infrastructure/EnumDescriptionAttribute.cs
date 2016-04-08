using System;

namespace BTTechnologies.MedScience.MVC.Infrastructure
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumDescriptionAttribute : Attribute
    {
        public string Value { get; private set; }
        
        public Type ResourcesType { get; private set; }
        public string ResourcesName { get; private set; }

        public EnumDescriptionAttribute(string value)
        {
            Value = value;
        }

        public EnumDescriptionAttribute(Type resourcesType, string resourcesName)
        {
            ResourcesType = resourcesType;
            ResourcesName = resourcesName;
        }
    }
}