using System;
using System.Web.Mvc;
using Seagull.Core;
using Seagull.Core.Data;
using Seagull.Core.Infrastructure;
using Seagull.Services.Users;

namespace Seagull.Web.Framework
{
    /// <summary>
    /// Represents filter attribute to validate user password expiration
    /// </summary>
    public class ValidatePasswordAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes
        /// </summary>
        /// <param name="filterContext">The filter context</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null || filterContext.HttpContext.Request == null)
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            var actionName = filterContext.ActionDescriptor.ActionName;
            if (string.IsNullOrEmpty(actionName) || actionName.Equals("ChangePassword", StringComparison.InvariantCultureIgnoreCase))
                return;

            var controllerName = filterContext.Controller.ToString();
            if (string.IsNullOrEmpty(controllerName) || controllerName.Equals("User", StringComparison.InvariantCultureIgnoreCase))
                return;

            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;

            //get current user
            var user = EngineContext.Current.Resolve<IWorkContext>().CurrentUser;


           //Skip Password Validation If There  Is Request to Change It. otherwise skip this.
            if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Equals("ChangePassword") && (filterContext.ActionDescriptor.ActionName.Equals("CreateOrEditModel") || filterContext.ActionDescriptor.ActionName.Equals("CreateOrEdit")))
                return;
            //check password expiration
            //if (user.PasswordIsExpired())
            //{
            //    var changePasswordUrl = new UrlHelper(filterContext.RequestContext).RouteUrl("UserChangePassword");
            //    filterContext.Result = new RedirectResult(changePasswordUrl);
            //}
        }
    }
}
