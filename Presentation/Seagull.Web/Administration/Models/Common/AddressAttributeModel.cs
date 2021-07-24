using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Common;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Localization;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Common
{
    [Validator(typeof(AddressAttributeValidator))]
    public partial class AddressAttributeModel : BaseSeagullEntityModel, ILocalizedModel<AddressAttributeLocalizedModel>
    {
        public AddressAttributeModel()
        {
            Locales = new List<AddressAttributeLocalizedModel>();
        }

        [SeagullResourceDisplayName("Admin.Address.AddressAttributes.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SeagullResourceDisplayName("Admin.Address.AddressAttributes.Fields.IsRequired")]
        public bool IsRequired { get; set; }

        [SeagullResourceDisplayName("Admin.Address.AddressAttributes.Fields.AttributeControlType")]
        public int AttributeControlTypeId { get; set; }
        [SeagullResourceDisplayName("Admin.Address.AddressAttributes.Fields.AttributeControlType")]
        [AllowHtml]
        public string AttributeControlTypeName { get; set; }

        [SeagullResourceDisplayName("Admin.Address.AddressAttributes.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }


        public IList<AddressAttributeLocalizedModel> Locales { get; set; }

    }

    public partial class AddressAttributeLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [SeagullResourceDisplayName("Admin.Address.AddressAttributes.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

    }
}