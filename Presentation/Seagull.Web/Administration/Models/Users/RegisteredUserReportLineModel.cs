using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Users
{
    public partial class RegisteredUserReportLineModel : BaseSeagullModel
    {
        [SeagullResourceDisplayName("Admin.Users.Reports.RegisteredUsers.Fields.Period")]
        public string Period { get; set; }

        [SeagullResourceDisplayName("Admin.Users.Reports.RegisteredUsers.Fields.Users")]
        public int Users { get; set; }
    }
}