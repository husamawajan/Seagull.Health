using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Linq;

namespace Seagull.Web.Framework
{
    public static class UrlHelperExtensions
    {
        public static string LogOn(this UrlHelper urlHelper, string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return urlHelper.Action("Login", "User", new { ReturnUrl = returnUrl });
            return urlHelper.Action("Login", "User");
        }

        public static string LogOff(this UrlHelper urlHelper, string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl))
                return urlHelper.Action("Logout", "User", new { ReturnUrl = returnUrl });
            return urlHelper.Action("Logout", "User");
        }
        #region Encrypt and Decrypt Url
        //Husam Majed Awajan
        //15-08-2016

        public static string ActionEncoded(this UrlHelper helper, string action)
        {
            return HttpUtility.HtmlEncode(helper.Action(action));
        }
        public static string ActionEncoded(this UrlHelper helper, string action, object routeValues)
        {
            return HttpUtility.HtmlEncode(helper.Action(action, routeValues));
        }
        public static string ActionEncodedCustom(object routeValues)
        {
            string queryString = string.Empty;
            if (routeValues != null)
            {
                RouteValueDictionary d = new RouteValueDictionary(routeValues);
                for (int i = 0; i < d.Keys.Count; i++)
                {
                    if (i > 0)
                    {
                        queryString += "?";
                    }
                    queryString += d.Keys.ElementAt(i) + "=" + d.Values.ElementAt(i);
                }
            }


            //What is Entity Framework??
            StringBuilder ancor = new StringBuilder();
            ancor.Append(Encrypt(queryString));
            return ancor.ToString();
        }
        private static string Encrypt(string plainText)
        {
            string key = "Seagull2017@@#";
            byte[] EncryptKey = { };
            byte[] IV = { 55, 34, 87, 64, 87, 195, 54, 21 };
            EncryptKey = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByte = Encoding.UTF8.GetBytes(plainText);
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, des.CreateEncryptor(EncryptKey, IV), CryptoStreamMode.Write);
            cStream.Write(inputByte, 0, inputByte.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray()).Replace('+', '*').Replace('/', '_');
        }
        #endregion
        //Url.Action("Create", "CustomerMasterAmendments", new { qs = UrlHelperExtensions.ActionEncodedCustom(new { CustomerId = _applicationRepository.Table.Where(a => a.Id == Appid).FirstOrDefault().CustId, ApplicationId = Appid, ProcessId = ProcessId }) });
    }
}
