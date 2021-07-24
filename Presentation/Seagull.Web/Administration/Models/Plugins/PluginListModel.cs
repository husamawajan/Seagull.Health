using System.Collections.Generic;
using System.Web.Mvc;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Plugins
{
    public partial class PluginListModel : BaseSeagullModel
    {
        public PluginListModel()
        {
            AvailableLoadModes = new List<SelectListItem>();
            AvailableGroups = new List<SelectListItem>();
        }

        [SeagullResourceDisplayName("Admin.Configuration.Plugins.LoadMode")]
        public int SearchLoadModeId { get; set; }
        [SeagullResourceDisplayName("Admin.Configuration.Plugins.Group")]
        public string SearchGroup { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Plugins.LoadMode")]
        public IList<SelectListItem> AvailableLoadModes { get; set; }
        [SeagullResourceDisplayName("Admin.Configuration.Plugins.Group")]
        public IList<SelectListItem> AvailableGroups { get; set; }
    }
}