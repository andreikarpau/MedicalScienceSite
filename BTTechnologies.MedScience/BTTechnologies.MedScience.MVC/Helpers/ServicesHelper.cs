using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BTTechnologies.MedScience.Domain.Entities;
using BTTechnologies.MedScience.MVC.App_LocalResources;
using BTTechnologies.MedScience.MVC.BTTClasses;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using BTTechnologies.MedScience.MVC.Infrastructure.Dependency;
using Ninject;

namespace BTTechnologies.MedScience.MVC.Helpers
{
    public static class ServicesHelper
    {
        private const int maxDirectoryNameLength = 100;

        public static IMembershipService GetMembershipService()
        {
            NinjectDependencyResolver resolver = new NinjectDependencyResolver();
            return resolver.Kernel.Get<IMembershipService>();
        }

        public static ISupportService GetSupportService()
        {
            NinjectDependencyResolver resolver = new NinjectDependencyResolver();
            return resolver.Kernel.Get<ISupportService>();
        }

        public static string GetAuthorFullName(Author author)
        {
            return author.Surname + " " + author.Name + " " + author.Patronymic;
        }

        public static List<BTTAjaxGridHelper.ChangeValueInformation> GetGridChangeBoolValueList()
        {
            return new List<BTTAjaxGridHelper.ChangeValueInformation>
                {
                    new BTTAjaxGridHelper.ChangeValueInformation(typeof(bool), Boolean.TrueString, MedSiteStrings.Yes),
                    new BTTAjaxGridHelper.ChangeValueInformation(typeof(bool), Boolean.FalseString, MedSiteStrings.No)
                };
        }

        public static string GetValidDirectoryName(string newDirectoryName)
        {
            if (string.IsNullOrEmpty(newDirectoryName))
                return string.Empty;

            string directoryName = newDirectoryName.Trim();

            directoryName = directoryName.Length > maxDirectoryNameLength
                ? directoryName.Substring(0, maxDirectoryNameLength)
                : directoryName;

            List<char> invalidChars = Path.GetInvalidFileNameChars().ToList();
            invalidChars.AddRange(Path.GetInvalidPathChars());
            invalidChars.Add('.');
            return invalidChars.Aggregate(directoryName, (current, invalidChar) => current.Replace(invalidChar, ' ')).Trim();
        }
    }
}