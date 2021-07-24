using System.Web.Routing;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Cms
{
    public partial class RenderWidgetModel : BaseSeagullModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}