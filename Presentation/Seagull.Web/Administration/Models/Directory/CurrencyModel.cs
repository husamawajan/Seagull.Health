using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Directory;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Localization;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Directory
{
    [Validator(typeof(CurrencyValidator))]
    public partial class CurrencyModel : BaseSeagullEntityModel, ILocalizedModel<CurrencyLocalizedModel>
    {
        public CurrencyModel()
        {
            Locales = new List<CurrencyLocalizedModel>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }
        [SeagullResourceDisplayName("Admin.Configuration.Currencies.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Currencies.Fields.CurrencyCode")]
        [AllowHtml]
        public string CurrencyCode { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Currencies.Fields.DisplayLocale")]
        [AllowHtml]
        public string DisplayLocale { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Currencies.Fields.Rate")]
        public decimal Rate { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Currencies.Fields.CustomFormatting")]
        [AllowHtml]
        public string CustomFormatting { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Currencies.Fields.Published")]
        public bool Published { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Currencies.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Currencies.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Currencies.Fields.IsPrimaryExchangeRateCurrency")]
        public bool IsPrimaryExchangeRateCurrency { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Currencies.Fields.IsPrimaryStoreCurrency")]
        public bool IsPrimaryStoreCurrency { get; set; }

        public IList<CurrencyLocalizedModel> Locales { get; set; }

        //store mapping
        [SeagullResourceDisplayName("Admin.ContentManagement.Blog.BlogPosts.Fields.LimitedToStores")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Currencies.Fields.RoundingType")]
        public int RoundingTypeId { get; set; }
    }

    public partial class CurrencyLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Currencies.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}