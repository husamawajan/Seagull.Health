using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Logging
{
    public partial class ActivityLogTypeModel : BaseSeagullEntityModel
    {
        [SeagullResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLogType.Fields.Name")]
        public string Name { get; set; }
        [SeagullResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLogType.Fields.Enabled")]
        public bool Enabled { get; set; }
    }
}