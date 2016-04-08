using System;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using BTTechnologies.MedScience.MVC.App_Start;
using BTTechnologies.MedScience.MVC.Infrastructure.Dependency;
using BTTechnologies.MedScience.MVC.Infrastructure.Sheduler;

namespace BTTechnologies.MedScience.MVC
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            
            DependencyResolver.SetResolver(new NinjectDependencyResolver()); 
            QuartzScheduler.Current.Run();
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                GenericPrincipal principal = new GenericPrincipal(new GenericIdentity(authTicket.Name, "Forms"), new []{string.Empty});
                Context.User = principal;
            }
        }
    }
}