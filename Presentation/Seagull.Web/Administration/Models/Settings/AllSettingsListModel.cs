using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Settings
{
    public class AllSettingsListModel : BaseSeagullModel
    {
        [SeagullResourceDisplayName("Admin.Configuration.Settings.AllSettings.SearchSettingName")]
        public string SearchSettingName { get; set; }
        [SeagullResourceDisplayName("Admin.Configuration.Settings.AllSettings.SearchSettingValue")]
        public string SearchSettingValue { get; set; }
    }
}