using System;
using System.Web.Mvc;
using Seagull.Core.Data;
using Seagull.Core.Domain.Security;
using Seagull.Core.Infrastructure;
using System.Web;
using System.Web.Helpers;

namespace Seagull.Web.Framework.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AdminAntiForgeryAttribute : FilterAttribute, IAuthorizationFilter
    {
        private readonly bool _ignore;

        /// <summary>
        /// Anti-forgery security attribute
        /// </summary>
        /// <param name="ignore">Pass false in order to ignore this security validation</param>
        public AdminAntiForgeryAttribute(bool ignore = false)
        {
            this._ignore = ignore;
        }
        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            if (_ignore)
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            //only POST requests
            if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
                return;

            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;
            var securitySettings = EngineContext.Current.Resolve<SecuritySettings>();
            if (!securitySettings.EnableXsrfProtectionForAdminArea)
                return;
            if (ValidateRequestHeader(filterContext.HttpContext.Request))
                return;
            var validator = new ValidateAntiForgeryTokenAttribute();
            validator.OnAuthorization(filterContext);
        }
        private bool ValidateRequestHeader(HttpRequestBase request)
        {
            bool Allow = false;
            string cookieToken = String.Empty;
            string formToken = String.Empty;
            string tokenValue = request.Headers["__RequestVerificationToken"];
            if (!String.IsNullOrEmpty(tokenValue))
            {
                string[] tokens = tokenValue.Split(':');
                if (tokens.Length == 2)
                {
                    cookieToken = tokens[0].Trim();
                    formToken = tokens[1].Trim();
                }
            }
            try
            {
                AntiForgery.Validate(cookieToken, formToken);
                Allow = true;
            }
            catch (Exception e)
            {
                Allow = false;
            }
            return Allow;
        }
        public static string GetAntiForgeryToken()
        {
            string cookieToken, formToken;
            AntiForgery.GetTokens(null, out cookieToken, out formToken);
            return cookieToken + ":" + formToken;
        }
    }
}
