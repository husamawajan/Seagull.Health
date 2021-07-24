using System.Web.Routing;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Web.Models.User
{
    public partial class ExternalAuthenticationMethodModel : BaseSeagullModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}