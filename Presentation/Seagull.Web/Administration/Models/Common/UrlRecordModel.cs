using System.Web.Mvc;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Common
{
    public partial class UrlRecordModel : BaseSeagullEntityModel
    {
        [SeagullResourceDisplayName("Admin.System.SeNames.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SeagullResourceDisplayName("Admin.System.SeNames.EntityId")]
        public int EntityId { get; set; }

        [SeagullResourceDisplayName("Admin.System.SeNames.EntityName")]
        public string EntityName { get; set; }

        [SeagullResourceDisplayName("Admin.System.SeNames.IsActive")]
        public bool IsActive { get; set; }

        [SeagullResourceDisplayName("Admin.System.SeNames.Language")]
        public string Language { get; set; }

        [SeagullResourceDisplayName("Admin.System.SeNames.Details")]
        public string DetailsUrl { get; set; }
    }
}