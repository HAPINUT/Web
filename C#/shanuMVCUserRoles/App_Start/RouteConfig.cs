using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace shanuMVCUserRoles
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute("Account", "Account/{action}/{id}", new { controller = "Account", action = "Index", id = UrlParameter.Optional }, new[] { "shanuMVCUserRoles.Controllers" });

            routes.MapRoute("Cart", "Cart/{action}/{id}", new { controller = "Cart", action = "Index", id = UrlParameter.Optional }, new[] { "shanuMVCUserRoles.Controllers" });

            routes.MapRoute("Shop", "Shop/{action}/{name}", new { controller = "Shop", action = "Index", name = UrlParameter.Optional }, new[] { "shanuMVCUserRoles.Controllers" });

            routes.MapRoute("Default", "", new { controller = "Shop", action = "Index" }, new[] { "shanuMVCUserRoles.Controllers" });


            //         routes.MapRoute(
            //	name: "Default",
            //	url: "{controller}/{action}/{id}",
            //	defaults: new { controller = "Shop", action = "Index", id = UrlParameter.Optional }
            //);
        }
    }
}
