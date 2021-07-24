using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Localization
{
    public class LanguageResourcesListModel : BaseSeagullModel
    {
        [SeagullResourceDisplayName("Admin.Configuration.Languages.Resources.SearchResourceName")]
        public string SearchResourceName { get; set; }
        [SeagullResourceDisplayName("Admin.Configuration.Languages.Resources.SearchResourceValue")]
        public string SearchResourceValue { get; set; }
    }
}