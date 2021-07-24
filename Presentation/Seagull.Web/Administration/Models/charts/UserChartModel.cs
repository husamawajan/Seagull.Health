using Seagull.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull.Admin.Models.charts
{
    public class UserChartModel : BaseSeagullEntityModel
    {
        
        public int Id { get; set; }
        public int UserId { get; set; }
        public string SQLstatement { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime updateDate { get; set; }
        public int Type { get; set; }
        public string Title { get; set; }
        public string Label { get; set; }
        public int ChartType { get; set; }
    }
}