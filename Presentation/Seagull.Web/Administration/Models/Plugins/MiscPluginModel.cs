using System.Web.Routing;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Plugins
{
    public partial class MiscPluginModel : BaseSeagullModel
    {
        public string FriendlyName { get; set; }

        public string ConfigurationActionName { get; set; }
        public string ConfigurationControllerName { get; set; }
        public RouteValueDictionary ConfigurationRouteValues { get; set; }
    }
}