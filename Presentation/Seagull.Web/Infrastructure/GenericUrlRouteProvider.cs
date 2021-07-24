using System.Web.Routing;
using Seagull.Web.Framework.Localization;
using Seagull.Web.Framework.Mvc.Routes;
using Seagull.Web.Framework.Seo;

namespace Seagull.Web.Infrastructure
{
    public partial class GenericUrlRouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //generic URLs
            routes.MapGenericPathRoute("GenericUrl",
                                       "{generic_se_name}",
                                       new {controller = "Common", action = "GenericUrl"},
                                       new[] {"Seagull.Web.Controllers"});

            //define this routes to use in UI views (in case if you want to customize some of them later)
            routes.MapLocalizedRoute("Product",
                                     "{SeName}",
                                     new { controller = "Product", action = "ProductDetails" },
                                     new[] {"Seagull.Web.Controllers"});

            routes.MapLocalizedRoute("Category",
                            "{SeName}",
                            new { controller = "Catalog", action = "Category" },
                            new[] { "Seagull.Web.Controllers" });

            routes.MapLocalizedRoute("Manufacturer",
                            "{SeName}",
                            new { controller = "Catalog", action = "Manufacturer" },
                            new[] { "Seagull.Web.Controllers" });

            routes.MapLocalizedRoute("Vendor",
                            "{SeName}",
                            new { controller = "Catalog", action = "Vendor" },
                            new[] { "Seagull.Web.Controllers" });
            
            routes.MapLocalizedRoute("NewsItem",
                            "{SeName}",
                            new { controller = "News", action = "NewsItem" },
                            new[] { "Seagull.Web.Controllers" });

            routes.MapLocalizedRoute("BlogPost",
                            "{SeName}",
                            new { controller = "Blog", action = "BlogPost" },
                            new[] { "Seagull.Web.Controllers" });

            routes.MapLocalizedRoute("Topic",
                            "{SeName}",
                            new { controller = "Topic", action = "TopicDetails" },
                            new[] { "Seagull.Web.Controllers" });



            //the last route. it's used when none of registered routes could be used for the current request
            //but in this case we cannot process non-registered routes (/controller/action)
            //routes.MapLocalizedRoute(
            //    "PageNotFound-Wildchar",
            //    "{*url}",
            //    new { controller = "Common", action = "PageNotFound" },
            //    new[] { "Seagull.Web.Controllers" });
        }

        public int Priority
        {
            get
            {
                //it should be the last route
                //we do not set it to -int.MaxValue so it could be overridden (if required)
                return -1000000;
            }
        }
    }
}
