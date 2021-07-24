using System.Web.Mvc;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Common
{
    public partial class UrlRecordListModel : BaseSeagullModel
    {
        [SeagullResourceDisplayName("Admin.System.SeNames.Name")]
        [AllowHtml]
        public string SeName { get; set; }
    }
}