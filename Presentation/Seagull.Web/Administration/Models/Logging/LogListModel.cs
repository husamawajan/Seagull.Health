using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Logging
{
    public partial class LogListModel : BaseSeagullModel
    {
        public LogListModel()
        {
            AvailableLogLevels = new List<SelectListItem>();
        }

        [SeagullResourceDisplayName("Admin.System.Log.List.CreatedOnFrom")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnFrom { get; set; }

        [SeagullResourceDisplayName("Admin.System.Log.List.CreatedOnTo")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnTo { get; set; }

        [SeagullResourceDisplayName("Admin.System.Log.List.Message")]
        [AllowHtml]
        public string Message { get; set; }

        [SeagullResourceDisplayName("Admin.System.Log.List.LogLevel")]
        public int LogLevelId { get; set; }


        public IList<SelectListItem> AvailableLogLevels { get; set; }
    }
}