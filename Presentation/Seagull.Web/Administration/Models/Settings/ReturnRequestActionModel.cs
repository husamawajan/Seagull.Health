using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Settings;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Localization;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Settings
{
    [Validator(typeof(ReturnRequestActionValidator))]
    public partial class ReturnRequestActionModel : BaseSeagullEntityModel, ILocalizedModel<ReturnRequestActionLocalizedModel>
    {
        public ReturnRequestActionModel()
        {
            Locales = new List<ReturnRequestActionLocalizedModel>();
        }

        [SeagullResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestActions.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestActions.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<ReturnRequestActionLocalizedModel> Locales { get; set; }
    }

    public partial class ReturnRequestActionLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestActions.Name")]
        [AllowHtml]
        public string Name { get; set; }

    }
}