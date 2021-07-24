using System.Web.Mvc;
using System.Web.Routing;
using Seagull.Core.Infrastructure;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Controllers;
using Seagull.Web.Framework.Security;
using Seagull.Web.Framework.Seo;

namespace Seagull.Web.Controllers
{
    [CheckAffiliate]
    [StoreClosed]
    [PublicStoreAllowNavigation]
    [LanguageSeoCode]
    [SeagullHttpsRequirement(SslRequirement.NoMatter)]
    [WwwRequirement]
    public abstract partial class BasePublicController : BaseController
    {
        protected virtual ActionResult InvokeHttp404()
        {
            // Call target Controller and pass the routeData.
            IController errorController = EngineContext.Current.Resolve<CommonController>();

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Common");
            routeData.Values.Add("action", "PageNotFound");

            errorController.Execute(new RequestContext(this.HttpContext, routeData));

            return new EmptyResult();
        }

    }
}
