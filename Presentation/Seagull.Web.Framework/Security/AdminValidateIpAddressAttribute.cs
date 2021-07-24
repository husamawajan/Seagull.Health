using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Seagull.Core;
using Seagull.Core.Domain.Security;
using Seagull.Core.Infrastructure;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using System.Web.Routing;
using System.Text;

namespace Seagull.Web.Framework.Security
{
    public class AdminValidateIpAddressAttribute : ActionFilterAttribute
    {
        //Husam Majed Awajan
        //15-08-2017
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
            CastRoutObjectAndDecodeUrl(filterContext);
            request = filterContext.HttpContext.Request;
            bool ok = false;
            var ipAddresses = EngineContext.Current.Resolve<SecuritySettings>().AdminAreaAllowedIpAddresses;
            if (ipAddresses != null && ipAddresses.Any())
            {
                var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                foreach (string ip in ipAddresses)
                    if (ip.Equals(webHelper.GetCurrentIpAddress(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        ok = true;
                        break;
                    }
            }
            else
            {
                //no restrictions
                ok = true;
            }

            if (!ok)
            {
                //ensure that it's not 'Access denied' page
                var webHelper = EngineContext.Current.Resolve<IWebHelper>();
                var thisPageUrl = webHelper.GetThisPageUrl(false);
                if (!thisPageUrl.StartsWith(string.Format("{0}admin/security/accessdenied", webHelper.GetStoreLocation()), StringComparison.InvariantCultureIgnoreCase))
                {
                    //redirect to 'Access denied' page
                    filterContext.Result = new RedirectResult(webHelper.GetStoreLocation() + "admin/security/accessdenied");
                    //filterContext.Result = RedirectToAction("AccessDenied", "Security");
                }
            }
        }
        public void CastRoutObjectAndDecodeUrl(ActionExecutingContext filterContext)
        {
            string[] allowNullCachActions = new string[] { "SetLanguage", "CountersCharts" , "ComprehensiveReport", "CurrentAssetsReport", "FinancialRatiosReport",
            "SubGridPrePaidAndPostPaidCountForOperatorsReport","RevenueReport","FollowupSurveyReport","PrePaidAndPostPaidCountForOperatorsReport",
                "FixedLineSubscribersCount","InternetSubscribersCount","GetFixedLineSubscribersCountSubGrid","QuarterlyPerformanceIndicators","AnnualPerformanceIndicators",
                "DynamicSurveyReport", "MarketShare","MobileTariff","AnnualComprehensiveReport","FinancialPerformanceIndicators","MobileTariffComprehensiveReport",
                "FixedTariffComprehensiveReport","ImportExcel","MailComprehensiveReport","AverageIndicatorsForMailsReport","ChangePassword", "LicensedCompaniesReport", "SubGridLicensedCompaniesReport"
            };
            if (allowNullCachActions.Contains(filterContext.ActionDescriptor.ActionName) )
                return;
            if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.Equals("ChangePassword") && (filterContext.ActionDescriptor.ActionName.Equals("CreateOrEditModel") || filterContext.ActionDescriptor.ActionName.Equals("CreateOrEdit")))
                return;
            if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "GenericGets" && filterContext.ActionDescriptor.ActionName == "NotifyForDelayReport")
                return;
            else if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Home" && filterContext.ActionDescriptor.ActionName == "Index")
                if (string.IsNullOrEmpty(HttpContext.Current.Request.QueryString.Get("seagull")))
                    return;
            try
            {
                Dictionary<string, object> decryptedParameters = new Dictionary<string, object>();
                if (HttpContext.Current.Request.HttpMethod == "GET" && filterContext.ActionParameters.Count > 0 && HttpContext.Current.Request.QueryString.Count == 0)
                {

                    if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "EconomyPlan" && filterContext.ActionDescriptor.ActionName == "PrepareEconomyPlan")
                        return;
                    filterContext.Result = new RedirectToRouteResult(
                                                   new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" } });
                    return;
                }
                else if (HttpContext.Current.Request.HttpMethod == "POST")
                {
                    return;
                }
                if (HttpContext.Current.Request.QueryString.Count > 0)
                {
                    if (string.IsNullOrEmpty(HttpContext.Current.Request.QueryString.Get("seagull")))
                    {
                        filterContext.Result = new RedirectToRouteResult(
                                                   new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" } });
                        return;
                    }
                    string encryptedQueryString = HttpContext.Current.Request.QueryString.Get("seagull").Replace('*', '+').Replace('_', '/');
                    string decrptedString = Decrypt(encryptedQueryString);
                    string[] paramsArrs = decrptedString.Split('?');
                    for (int i = 0; i < paramsArrs.Length; i++)
                    {
                        string[] paramArr = paramsArrs[i].Split('=');
                        decryptedParameters.Add(paramArr[0], paramArr[1]);
                    }
                }
                for (int i = 0; i < decryptedParameters.Count; i++)
                {
                    var type = filterContext.ActionDescriptor.GetParameters()[i].ParameterType; //get type expected
                    //Covert Object To It's Original Datatype
                    filterContext.ActionParameters[decryptedParameters.Keys.ElementAt(i)] = Convert.ChangeType(decryptedParameters.Values.ElementAt(i), type);
                }
            }
            catch (Exception r)
            {
                filterContext.Result = new RedirectToRouteResult(
                                                  new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" } });
                return;
            }
        }
        private string Decrypt(string encryptedText)
        {
            string key = "Seagull2017@@#";
            byte[] DecryptKey = { };
            byte[] IV = { 55, 34, 87, 64, 87, 195, 54, 21 };
            byte[] inputByte = new byte[encryptedText.Length];
            DecryptKey = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByte = Convert.FromBase64String(encryptedText);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(DecryptKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByte, 0, inputByte.Length);
            cs.FlushFinalBlock();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }
    }
}
