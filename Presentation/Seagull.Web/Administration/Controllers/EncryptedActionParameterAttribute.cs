using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Seagull.Admin.Controllers
{
    //Husam Majed Awajan
    //15-08-2017
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EncryptedActionParameterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            //CastRoutObjectAndDecodeUrl(filterContext);
            //base.OnActionExecuting(filterContext);

        }
       
    }
}
