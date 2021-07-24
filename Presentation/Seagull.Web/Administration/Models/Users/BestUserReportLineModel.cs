using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Users
{
    public partial class BestUserReportLineModel : BaseSeagullModel
    {
        public int UserId { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Reports.BestBy.Fields.User")]
        public string UserName { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Reports.BestBy.Fields.OrderTotal")]
        public string OrderTotal { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Reports.BestBy.Fields.OrderCount")]
        public decimal OrderCount { get; set; }
    }
}