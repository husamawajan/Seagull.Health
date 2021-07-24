using System;
using System.Web.Mvc;
using Seagull.Core;
using Seagull.Core.Data;
using Seagull.Core.Infrastructure;
using Seagull.Services.Users;

namespace Seagull.Web.Framework
{
    public class UserLastActivityAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            if (filterContext == null || filterContext.HttpContext == null || filterContext.HttpContext.Request == null)
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            //only GET requests
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                return;

            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            var user = workContext.CurrentUser;

            //update last activity date
            if (user.LastActivityDateUtc.AddMinutes(1.0) < DateTime.UtcNow)
            {
                var userService = EngineContext.Current.Resolve<IUserService>();
                user.LastActivityDateUtc = DateTime.UtcNow;
                userService.UpdateUser(user);
            }
        }
    }
}
