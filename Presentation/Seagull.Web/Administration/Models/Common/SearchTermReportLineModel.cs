using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Common
{
    public partial class SearchTermReportLineModel : BaseSeagullModel
    {
        [SeagullResourceDisplayName("Admin.SearchTermReport.Keyword")]
        public string Keyword { get; set; }

        [SeagullResourceDisplayName("Admin.SearchTermReport.Count")]
        public int Count { get; set; }
    }
}
