using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Seagull.Core.Domain.Chart;

namespace Seagull.Data.Mapping.Chart 
{
    public class UserChartMap : SeagullEntityTypeConfiguration<UserChart>
    {
        public UserChartMap()
        {
            this.ToTable("UserChart");
            this.HasKey(c => c.Id);
            //this.Property(c => c.UserId);
            //this.Property(c => c.SQLstatement);
            //this.Property(c => c.CreateDate);
            //this.Property(c => c.UpdateDate);
           
        }
    }
}
