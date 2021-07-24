using System;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Logging
{
    public partial class ActivityLogModel : BaseSeagullEntityModel
    {
        [SeagullResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.ActivityLogType")]
        public string ActivityLogTypeName { get; set; }
        [SeagullResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.User")]
        public int UserId { get; set; }
        [SeagullResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.User")]
        public string UserEmail { get; set; }
        [SeagullResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.Comment")]
        public string Comment { get; set; }
        [SeagullResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [SeagullResourceDisplayName("Admin.Users.Users.ActivityLog.IpAddress")]
        public string IpAddress { get; set; }

        public string CreatedOnStr { get; set; }
    }
}
