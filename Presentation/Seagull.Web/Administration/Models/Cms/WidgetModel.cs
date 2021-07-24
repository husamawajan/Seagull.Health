using System.Web.Mvc;
using System.Web.Routing;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Cms
{
    public partial class WidgetModel : BaseSeagullModel
    {
        [SeagullResourceDisplayName("Admin.ContentManagement.Widgets.Fields.FriendlyName")]
        [AllowHtml]
        public string FriendlyName { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Widgets.Fields.SystemName")]
        [AllowHtml]
        public string SystemName { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Widgets.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [SeagullResourceDisplayName("Admin.ContentManagement.Widgets.Fields.IsActive")]
        public bool IsActive { get; set; }
        

        public string ConfigurationActionName { get; set; }
        public string ConfigurationControllerName { get; set; }
        public RouteValueDictionary ConfigurationRouteValues { get; set; }
    }
}