using System;
using System.Linq;
using System.Web.Mvc;
using Seagull.Services.Common;

namespace Seagull.Admin.Extensions
{
    /// <summary>
    /// Parser helper
    /// </summary>
    public static class AttributeParserHelper
    {
        public static string ParseCustomAddressAttributes(this FormCollection form,
            IAddressAttributeParser addressAttributeParser,
            IAddressAttributeService addressAttributeService)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            string attributesXml = "";
            var attributes = addressAttributeService.GetAllAddressAttributes();
            foreach (var attribute in attributes)
            {
                string controlId = string.Format("address_attribute_{0}", attribute.Id);
   
            }

            return attributesXml;
        }
    }
}

