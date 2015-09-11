using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TvTamer.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "This Week",
                url: "this-week",
                defaults: new { controller = "Episode", action = "Calendar", id = UrlParameter.Optional}
            );

            routes.MapRoute(
                name: "Wanted Episodes",
                url: "wanted",
                defaults: new { controller = "Episode", action = "WantedEpisodes", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SeriesView",
                url: "show/details/{id}/{slug}",
                defaults: new { controller = "Series", action = "Details", id = UrlParameter.Optional, slug = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
