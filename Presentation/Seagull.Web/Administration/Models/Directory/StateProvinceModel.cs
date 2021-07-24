using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Directory;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Localization;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Directory
{
    [Validator(typeof(StateProvinceValidator))]
    public partial class StateProvinceModel : BaseSeagullEntityModel, ILocalizedModel<StateProvinceLocalizedModel>
    {
        public StateProvinceModel()
        {
            Locales = new List<StateProvinceLocalizedModel>();
        }
        public int CountryId { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Countries.States.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Countries.States.Fields.Abbreviation")]
        [AllowHtml]
        public string Abbreviation { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Countries.States.Fields.Published")]
        public bool Published { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Countries.States.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<StateProvinceLocalizedModel> Locales { get; set; }
    }

    public partial class StateProvinceLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }
        
        [SeagullResourceDisplayName("Admin.Configuration.Countries.States.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}