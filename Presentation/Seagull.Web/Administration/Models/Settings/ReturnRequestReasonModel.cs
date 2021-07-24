using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Settings;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Localization;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Settings
{
    [Validator(typeof(ReturnRequestReasonValidator))]
    public partial class ReturnRequestReasonModel : BaseSeagullEntityModel, ILocalizedModel<ReturnRequestReasonLocalizedModel>
    {
        public ReturnRequestReasonModel()
        {
            Locales = new List<ReturnRequestReasonLocalizedModel>();
        }

        [SeagullResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestReasons.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestReasons.DisplayOrder")]
        public int DisplayOrder { get; set; }

        public IList<ReturnRequestReasonLocalizedModel> Locales { get; set; }
    }

    public partial class ReturnRequestReasonLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Settings.Order.ReturnRequestReasons.Name")]
        [AllowHtml]
        public string Name { get; set; }

    }
}