using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Localization;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Localization
{
    [Validator(typeof(LanguageValidator))]
    public partial class LanguageModel : BaseSeagullEntityModel
    {
        public LanguageModel()
        {
            AvailableCurrencies = new List<SelectListItem>();
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        [SeagullResourceDisplayName("Admin.Configuration.Languages.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Languages.Fields.LanguageCulture")]
        [AllowHtml]
        public string LanguageCulture { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Languages.Fields.UniqueSeoCode")]
        [AllowHtml]
        public string UniqueSeoCode { get; set; }
        
        //flags
        [SeagullResourceDisplayName("Admin.Configuration.Languages.Fields.FlagImageFileName")]
        public string FlagImageFileName { get; set; }


        [SeagullResourceDisplayName("Admin.Configuration.Languages.Fields.Rtl")]
        public bool Rtl { get; set; }

        //default currency
        [SeagullResourceDisplayName("Admin.Configuration.Languages.Fields.DefaultCurrency")]
        [AllowHtml]
        public int DefaultCurrencyId { get; set; }
        public IList<SelectListItem> AvailableCurrencies { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Languages.Fields.Published")]
        public bool Published { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Languages.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public string EncId { get; set; } //EncryptUrl
        //store mapping
        [SeagullResourceDisplayName("Admin.Configuration.Languages.Fields.LimitedToStores")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        // search
        public LanguageResourcesListModel Search { get; set; }
    }
}