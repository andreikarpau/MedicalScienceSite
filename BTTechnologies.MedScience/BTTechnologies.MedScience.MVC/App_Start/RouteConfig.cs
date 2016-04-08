using System.Web.Mvc;
using System.Web.Routing;

namespace BTTechnologies.MedScience.MVC.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
            routes.MapRoute("IdWithValueRote", "{controller}/{action}/{id}/{value}");
            routes.MapRoute("IdWithTwoValuesRote", "{controller}/{action}/{id}/{value}/{value2}");
        }
    }
}