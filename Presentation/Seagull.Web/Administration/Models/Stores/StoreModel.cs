using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Stores;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Localization;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Stores
{
    [Validator(typeof(StoreValidator))]
    public partial class StoreModel : BaseSeagullEntityModel, ILocalizedModel<StoreLocalizedModel>
    {
        public StoreModel()
        {
            Locales = new List<StoreLocalizedModel>();
            AvailableLanguages = new List<SelectListItem>();
        }

        [SeagullResourceDisplayName("Admin.Configuration.Stores.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Stores.Fields.Url")]
        [AllowHtml]
        public string Url { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Stores.Fields.SslEnabled")]
        public virtual bool SslEnabled { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Stores.Fields.SecureUrl")]
        [AllowHtml]
        public virtual string SecureUrl { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Stores.Fields.Hosts")]
        [AllowHtml]
        public string Hosts { get; set; }

        //default language
        [SeagullResourceDisplayName("Admin.Configuration.Stores.Fields.DefaultLanguage")]
        [AllowHtml]
        public int DefaultLanguageId { get; set; }
        public IList<SelectListItem> AvailableLanguages { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Stores.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Stores.Fields.CompanyName")]
        [AllowHtml]
        public string CompanyName { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Stores.Fields.CompanyAddress")]
        [AllowHtml]
        public string CompanyAddress { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Stores.Fields.CompanyPhoneNumber")]
        [AllowHtml]
        public string CompanyPhoneNumber { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Stores.Fields.CompanyVat")]
        [AllowHtml]
        public string CompanyVat { get; set; }


        public IList<StoreLocalizedModel> Locales { get; set; }
    }

    public partial class StoreLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Stores.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}