using System.Collections.Generic;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Web.Models.Common
{
    public partial class AddressAttributeModel : BaseSeagullEntityModel
    {
        public AddressAttributeModel()
        {
            Values = new List<AddressAttributeValueModel>();
        }

        public string Name { get; set; }

        public bool IsRequired { get; set; }

        /// <summary>
        /// Default value for textboxes
        /// </summary>
        public string DefaultValue { get; set; }

        public IList<AddressAttributeValueModel> Values { get; set; }
    }

    public partial class AddressAttributeValueModel : BaseSeagullEntityModel
    {
        public string Name { get; set; }

        public bool IsPreSelected { get; set; }
    }
}