using System.Web.Mvc;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Settings
{
    public partial class SortOptionModel : BaseSeagullEntityModel
    {
        [SeagullResourceDisplayName("Admin.Configuration.Settings.Catalog.SortOptions.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Settings.Catalog.SortOptions.IsActive")]
        public bool IsActive { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.Settings.Catalog.SortOptions.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}