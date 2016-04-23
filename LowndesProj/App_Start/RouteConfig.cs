using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LowndesProj {
    public class RouteConfig {
        public static void RegisterRoutes( RouteCollection routes ) {
            routes.IgnoreRoute( "{resource}.axd/{*pathInfo}" );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{func}/{id}",
                defaults: new { controller = "Nomination", action = "Index", func = UrlParameter.Optional, id = UrlParameter.Optional },
                namespaces: new string[] { "LowndesProj.Controllers" }
            );
        }
    }
}
