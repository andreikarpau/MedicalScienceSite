using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BTTechnologies.MedScience.MVC.ControllersServices.Abstract;
using BTTechnologies.MedScience.MVC.Infrastructure.Dependency;
using Ninject;

namespace BTTechnologies.MedScience.MVC.Infrastructure.Security
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class PermissionAttribute : AuthorizeAttribute
    {
        private readonly Privilege[] privileges;
        private readonly NinjectDependencyResolver resolver = new NinjectDependencyResolver();

        public PermissionAttribute(params Privilege[] allowPrivileges)
        {
            privileges = allowPrivileges;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.Request.IsAuthenticated)
                return false;

            if (privileges == null || !privileges.Any())
            {
                return httpContext.Request.IsAuthenticated;
            }

            return resolver.Kernel.Get<IMembershipService>().UserHasPrivilege(httpContext, privileges);
        }
    }
}