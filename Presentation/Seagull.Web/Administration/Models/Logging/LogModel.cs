using System;
using System.Web.Mvc;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Logging
{
    public partial class LogModel : BaseSeagullEntityModel
    {
        [SeagullResourceDisplayName("Admin.System.Log.Fields.LogLevel")]
        public string LogLevel { get; set; }

        [SeagullResourceDisplayName("Admin.System.Log.Fields.ShortMessage")]
        [AllowHtml]
        public string ShortMessage { get; set; }

        [SeagullResourceDisplayName("Admin.System.Log.Fields.FullMessage")]
        [AllowHtml]
        public string FullMessage { get; set; }

        [SeagullResourceDisplayName("Admin.System.Log.Fields.IPAddress")]
        [AllowHtml]
        public string IpAddress { get; set; }

        [SeagullResourceDisplayName("Admin.System.Log.Fields.User")]
        public int? UserId { get; set; }
        [SeagullResourceDisplayName("Admin.System.Log.Fields.User")]
        public string UserEmail { get; set; }

        [SeagullResourceDisplayName("Admin.System.Log.Fields.PageURL")]
        [AllowHtml]
        public string PageUrl { get; set; }

        [SeagullResourceDisplayName("Admin.System.Log.Fields.ReferrerURL")]
        [AllowHtml]
        public string ReferrerUrl { get; set; }

        [SeagullResourceDisplayName("Admin.System.Log.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
    }
}