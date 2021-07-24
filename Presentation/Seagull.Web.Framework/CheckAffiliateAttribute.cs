using System;
using System.Web;
using System.Web.Mvc;
using Seagull.Core;
using Seagull.Core.Infrastructure;
using Seagull.Services.Users;

namespace Seagull.Web.Framework
{
    public class CheckAffiliateAttribute : ActionFilterAttribute
    {
        private const string AFFILIATE_ID_QUERY_PARAMETER_NAME = "affiliateid";
        private const string AFFILIATE_FRIENDLYURLNAME_QUERY_PARAMETER_NAME = "affiliate";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;


            if (request.QueryString != null)
            {
                //try to find by ID ("affiliateId" parameter)
                if (request.QueryString[AFFILIATE_ID_QUERY_PARAMETER_NAME] != null)
                {

                }
                //try to find by friendly name ("affiliate" parameter)
                else if (request.QueryString[AFFILIATE_FRIENDLYURLNAME_QUERY_PARAMETER_NAME] != null)
                {
                    var friendlyUrlName = request.QueryString[AFFILIATE_FRIENDLYURLNAME_QUERY_PARAMETER_NAME];
              
                }
            }



        }
    }
}
