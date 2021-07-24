using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seagull.Core.Domain.Chart
{
    public class UserChart : BaseEntity
    {
        //the entity
        public int Id { get; set; }
        public int UserId { get; set; }
        public string SQLstatement { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime?  UpdateDate { get; set;}
        public int? Type { get; set; }
        public string Title { get; set; }
        public string Label { get; set; }
        public string GridModel { get; set; }
        public int? ChartType { get ; set; }
    }
}
