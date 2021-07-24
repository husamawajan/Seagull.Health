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
    [Validator(typeof(CountryValidator))]
    public partial class CountryModel : BaseSeagullEntityModel, ILocalizedModel<CountryLocalizedModel>
    {
        public CountryModel()
        {
            Locales = new List<CountryLocalizedModel>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }
        [SeagullResourceDisplayName("Admin.Configuration.Countries.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Countries.Fields.AllowsBilling")]
        public bool AllowsBilling { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Countries.Fields.AllowsShipping")]
        public bool AllowsShipping { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Countries.Fields.TwoLetterIsoCode")]
        [AllowHtml]
        public string TwoLetterIsoCode { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Countries.Fields.ThreeLetterIsoCode")]
        [AllowHtml]
        public string ThreeLetterIsoCode { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Countries.Fields.NumericIsoCode")]
        public int NumericIsoCode { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Countries.Fields.SubjectToVat")]
        public bool SubjectToVat { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Countries.Fields.Published")]
        public bool Published { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Countries.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }




        [SeagullResourceDisplayName("Admin.Configuration.Countries.Fields.NumberOfStates")]
        public int NumberOfStates { get; set; }

        public IList<CountryLocalizedModel> Locales { get; set; }


        //store mapping
        [SeagullResourceDisplayName("Admin.Configuration.Countries.Fields.LimitedToStores")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }

    public partial class CountryLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Countries.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}