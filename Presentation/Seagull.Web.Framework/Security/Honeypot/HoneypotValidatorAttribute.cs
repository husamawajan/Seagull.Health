using System;
using System.Web.Mvc;
using Seagull.Core;
using Seagull.Core.Domain.Security;
using Seagull.Core.Infrastructure;
using Seagull.Services.Logging;

namespace Seagull.Web.Framework.Security.Honeypot
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class HoneypotValidatorAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            var securitySettings = EngineContext.Current.Resolve<SecuritySettings>();
            if (securitySettings.HoneypotEnabled)
            {
                string inputValue = filterContext.HttpContext.Request.Form[securitySettings.HoneypotInputName];

                var isBot = !String.IsNullOrWhiteSpace(inputValue);
                if (isBot)
                {
                    var logger = EngineContext.Current.Resolve<ILogger>();
                    logger.Warning("A bot detected. Honeypot.");

                    //filterContext.Result = new HttpUnauthorizedResult();
                    var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                    string url = webHelper.GetThisPageUrl(true);
                    filterContext.Result = new RedirectResult(url);
                }
            }
        }
    }
}
