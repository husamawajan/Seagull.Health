using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Users
{
    public partial class UserReportsModel : BaseSeagullModel
    {
        public BestUsersReportModel BestUsersByOrderTotal { get; set; }
        public BestUsersReportModel BestUsersByNumberOfOrders { get; set; }
    }
}