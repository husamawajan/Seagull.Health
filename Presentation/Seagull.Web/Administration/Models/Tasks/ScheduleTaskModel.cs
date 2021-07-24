using System.Web.Mvc;
using FluentValidation.Attributes;
using Seagull.Admin.Validators.Tasks;
using Seagull.Web.Framework;
using Seagull.Web.Framework.Mvc;

namespace Seagull.Admin.Models.Tasks
{
    [Validator(typeof(ScheduleTaskValidator))]
    public partial class ScheduleTaskModel : BaseSeagullEntityModel
    {
        [SeagullResourceDisplayName("Admin.System.ScheduleTasks.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [SeagullResourceDisplayName("Admin.System.ScheduleTasks.Seconds")]
        public int Seconds { get; set; }

        [SeagullResourceDisplayName("Admin.System.ScheduleTasks.Enabled")]
        public bool Enabled { get; set; }

        [SeagullResourceDisplayName("Admin.System.ScheduleTasks.StopOnError")]
        public bool StopOnError { get; set; }

        [SeagullResourceDisplayName("Admin.System.ScheduleTasks.LastStart")]
        public string LastStartUtc { get; set; }

        [SeagullResourceDisplayName("Admin.System.ScheduleTasks.LastEnd")]
        public string LastEndUtc { get; set; }

        [SeagullResourceDisplayName("Admin.System.ScheduleTasks.LastSuccess")]
        public string LastSuccessUtc { get; set; }
    }
}