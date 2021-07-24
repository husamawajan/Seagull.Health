using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seagull.Data.Mapping.Notify
{
    public class GeneralDelayReportNotification
    {
        public GeneralDelayReportNotification()
        {
            _delayReportNotification = new List<DelayReportNotification>();
        }
        public int CountNotification { get; set; }
        public DateTime UserLastSeen { get; set; }
        public List<DelayReportNotification> _delayReportNotification { get; set; }
        public List<DelayTaskNotification> _delayTaskNotification { get; set; }
    }
    public class DelayReportNotification
    {
        public int? Id { get; set; }
        public int EntityId { get; set; }
        public string EntityName { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public string Year { get; set; }
        public string Msg { get; set; }
        public DateTime CreatedDate { get; set; }

    }
    public class DelayTaskNotification
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public string EntityName { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public string TaskName { get; set; }

    }

    //public class DelayReportNotification
    //{
    //    public int? Id { get; set; }
    //    public int EntityId { get; set; }
    //    public string EntityName { get; set; }
    //    public int Month { get; set; }
    //    public string MonthName { get; set; }

    //}

}
