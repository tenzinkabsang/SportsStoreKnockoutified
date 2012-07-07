using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Binders;
using SportsStore.WebUI.Infrastructure;

namespace SportsStore.WebUI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                null,
                "", // Only matches the empty URL (i.e. /)
                new {controller = "Product", action = "List", category = (string) null, page = 1}
                );

            routes.MapRoute(
                null,
                "Page{page}", // Matches /Page2, /Page123, but not PageXYZ
                new {controller = "Product", action = "List", category = (string) null},
                new {page = @"\d+"} // Constraints: page must be numerical
                );

            routes.MapRoute(
                null,
                "{category}", // Matches /Football, or /AnythingWithNoSlash
                new {controller = "Product", action = "List", page = 1}
                );

            routes.MapRoute(
                null,
                "{category}/Page{page}", // Mathces /Football/Page123
                new {controller = "Product", action = "List"}, // Defaults
                new {page = @"\d+"}
                );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Product", action = "List", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ControllerBuilder.Current.SetControllerFactory(new NinjectControllerFactory());
            ModelBinders.Binders.Add(typeof(Cart), new CartModelBinder());
        }
    }
}