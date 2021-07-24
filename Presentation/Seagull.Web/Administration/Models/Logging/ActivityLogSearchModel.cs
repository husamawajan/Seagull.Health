using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Logging
{
    public partial class ActivityLogSearchModel : BaseSeagullModel
    {
        public ActivityLogSearchModel()
        {
            ActivityLogType = new List<SelectListItem>();
        }
        [SeagullResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.CreatedOnFrom")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnFrom { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.CreatedOnTo")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnTo { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.ActivityLogType")]
        public int ActivityLogTypeId { get; set; }

        [SeagullResourceDisplayName("Admin.Configuration.ActivityLog.ActivityLog.Fields.ActivityLogType")]
        public IList<SelectListItem> ActivityLogType { get; set; }
        
        [SeagullResourceDisplayName("Admin.Users.Users.ActivityLog.IpAddress")]
        public string IpAddress { get; set; }
    }
}