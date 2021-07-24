using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Common;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Localization;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Common
{
    [Validator(typeof(AddressAttributeValueValidator))]
    public partial class AddressAttributeValueModel : BaseSeagullEntityModel, ILocalizedModel<AddressAttributeValueLocalizedModel>
    {
        public AddressAttributeValueModel()
        {
            Locales = new List<AddressAttributeValueLocalizedModel>();
        }

        public int AddressAttributeId { get; set; }

        [SeagullResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SeagullResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [SeagullResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.DisplayOrder")]
        public int DisplayOrder {get;set;}

        public IList<AddressAttributeValueLocalizedModel> Locales { get; set; }

    }

    public partial class AddressAttributeValueLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [SeagullResourceDisplayName("Admin.Address.AddressAttributes.Values.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}