using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json.Converters;
using Seagull.Core;
using Seagull.Core.Domain.Common;
using Seagull.Core.Infrastructure;
using Seagull.Services.Localization;
using Seagull.Web.Framework.Controllers;
using Seagull.Web.Framework.Mvc;
using Seagull.Web.Framework.Security;
using System.Collections.Generic;
using System.Linq;
using Seagull.Web.Framework;
using Seagull.Admin.Helpers;
namespace Seagull.Admin.Controllers
{
    [SeagullHttpsRequirement(SslRequirement.Yes)]
    //[EncryptedActionParameterAttribute]
    [AdminValidateIpAddress]
    [AdminAuthorize]
    [AdminAntiForgery]
    public abstract partial class BaseAdminController : BaseController
    {
        /// <summary>
        /// Initialize controller
        /// </summary>
        /// <param name="requestContext">Request context</param>
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            //set work context to admin mode
            EngineContext.Current.Resolve<IWorkContext>().IsAdmin = true;

            base.Initialize(requestContext);
        }
        
        /// <summary>
        /// On exception
        /// </summary>
        /// <param name="filterContext">Filter context</param>
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception != null)
                LogException(filterContext.Exception);
            base.OnException(filterContext);
        }
        
        /// <summary>
        /// Access denied view
        /// </summary>
        /// <returns>Access denied view</returns>
        protected virtual ActionResult AccessDeniedView()
        {
            //return new HttpUnauthorizedResult();
            return RedirectToAction("AccessDenied", "Security", new { seagull = UrlHelperExtensions.ActionEncodedCustom(new { pageUrl = this.Request.RawUrl }) });//pageUrl = this.Request.RawUrl });
        }
        /// <summary>
        /// Access denied view as json
        /// </summary>
        /// <returns>Access denied view as json</returns>
        protected JsonResult AccessDeniedJson(string msg = "")
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            //return new HttpUnauthorizedResult();
            JsonResultHelper data = new JsonResultHelper
            {
                Access = false,
                Msg = new List<string> { string.IsNullOrEmpty(msg) ?  localizationService.GetResource("Admin.AccessDenied.Description") : msg }
            };
            return Json(data);
        }

        /// <summary>
        /// Access denied json data for kendo grid
        /// </summary>
        /// <returns>Access denied json data</returns>
        protected JsonResult AccessDeniedKendoGridJson()
        {
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            
            return ErrorForKendoGridJson(localizationService.GetResource("Admin.AccessDenied.Description"));
        }

        /// <summary>
        /// Save selected TAB name
        /// </summary>
        /// <param name="tabName">Tab name to save; empty to automatically detect it</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        protected virtual void SaveSelectedTabName(string tabName = "", bool persistForTheNextRequest = true)
        {
            //keep this method synchronized with
            //"GetSelectedTabName" method of \Seagull.Web.Framework\HtmlExtensions.cs
            if (string.IsNullOrEmpty(tabName))
            {
                tabName = this.Request.Form["selected-tab-name"];
            }
            
            if (!string.IsNullOrEmpty(tabName))
            {
                const string dataKey = "Seagull.selected-tab-name";
                if (persistForTheNextRequest)
                {
                    TempData[dataKey] = tabName;
                }
                else
                {
                    ViewData[dataKey] = tabName;
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="T:System.Web.Mvc.JsonResult"/> object that serializes the specified object to JavaScript Object Notation (JSON) format using the content type, content encoding, and the JSON request behavior.
        /// </summary>
        /// 
        /// <returns>
        /// The result object that serializes the specified object to JSON format.
        /// </returns>
        /// <param name="data">The JavaScript object graph to serialize.</param>
        /// <param name="contentType">The content type (MIME type).</param>
        /// <param name="contentEncoding">The content encoding.</param>
        /// <param name="behavior">The JSON request behavior</param>
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            //Json fix issue with dates in KendoUI grid
            //use json with IsoDateTimeConverter
            var result = EngineContext.Current.Resolve<AdminAreaSettings>().UseIsoDateTimeConverterInJson
                ? new ConverterJsonResult(new IsoDateTimeConverter()) : new JsonResult();

            result.Data = data;
            result.ContentType = contentType;
            result.ContentEncoding = contentEncoding;
            result.JsonRequestBehavior = behavior;

            //Json fix for admin area
            //sometime our entities have big text values returned (e.g. product desriptions)
            //of course, we can set and return them as "empty" (we already do it so). Furthermore, it's a perfoemance optimization
            //but it's better to avoid exceptions for other entities and allow maximum JSON length
            result.MaxJsonLength = int.MaxValue;

            return result;
            //return base.Json(data, contentType, contentEncoding, behavior);
        }
        // This method helps to get the error information from the MVC "ModelState".
        // We can not directly send the ModelState to the client in Json. The "ModelState"
        // object has some circular reference that prevents it to be serialized to Json.
        public Dictionary<string, object> GetErrorsFromModelState()
        {
            var errors = new Dictionary<string, object>();
            // Only send the errors to the client.
            ModelState.AsQueryable().Where(f => f.Value.Errors.Count > 0).Select(a=>a.Key).ToList().ForEach(a => errors[a.Split('.')[1]] = ModelState[a].Errors);
            return errors;
        }
    }
}
