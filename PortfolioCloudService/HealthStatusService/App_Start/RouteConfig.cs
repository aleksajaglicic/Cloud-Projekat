﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HealthStatusService
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "HealthCheck",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "HealthCheck", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
