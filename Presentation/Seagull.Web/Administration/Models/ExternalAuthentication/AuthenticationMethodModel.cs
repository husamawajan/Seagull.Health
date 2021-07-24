using System.Web.Mvc;
using System.Web.Routing;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.ExternalAuthentication
{
    public partial class AuthenticationMethodModel : BaseSeagullModel
    {
        [SeagullResourceDisplayName("Admin.Configuration.ExternalAuthenticationMethods.Fields.FriendlyName")]
        [AllowHtml]
        public string FriendlyName { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.ExternalAuthenticationMethods.Fields.SystemName")]
        [AllowHtml]
        public string SystemName { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.ExternalAuthenticationMethods.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.ExternalAuthenticationMethods.Fields.IsActive")]
        public bool IsActive { get; set; }



        public string ConfigurationActionName { get; set; }
        public string ConfigurationControllerName { get; set; }
        public RouteValueDictionary ConfigurationRouteValues { get; set; }
    }
}