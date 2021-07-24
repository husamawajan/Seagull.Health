using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Settings;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Settings
{
    [Validator(typeof(SettingValidator))]
    public partial class SettingModel : BaseSeagullEntityModel
    {
        [SeagullResourceDisplayName("Admin.Configuration.Settings.AllSettings.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Settings.AllSettings.Fields.Value")]
        [AllowHtml]
        public string Value { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Settings.AllSettings.Fields.StoreName")]
        public string Store { get; set; }
        public int StoreId { get; set; }
    }
}