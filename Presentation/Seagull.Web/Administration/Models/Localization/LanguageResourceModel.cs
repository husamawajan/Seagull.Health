using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Localization;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Localization
{
    [Validator(typeof(LanguageResourceValidator))]
    public partial class LanguageResourceModel : BaseSeagullEntityModel
    {
        [SeagullResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Languages.Resources.Fields.Value")]
        [AllowHtml]
        public string Value { get; set; }

        public int LanguageId { get; set; }
    }
}