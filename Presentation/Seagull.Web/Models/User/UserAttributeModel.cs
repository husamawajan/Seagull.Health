using System.Collections.Generic;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Web.Models.User
{
    public partial class UserAttributeModel : BaseSeagullEntityModel
    {
        public UserAttributeModel()
        {
            Values = new List<UserAttributeValueModel>();
        }

        public string Name { get; set; }

        public bool IsRequired { get; set; }

        /// <summary>
        /// Default value for textboxes
        /// </summary>
        public string DefaultValue { get; set; }

        public IList<UserAttributeValueModel> Values { get; set; }

    }

    public partial class UserAttributeValueModel : BaseSeagullEntityModel
    {
        public string Name { get; set; }

        public bool IsPreSelected { get; set; }
    }
}