using Seagull.Core.Domain.Tasks;

namespace Seagull.Data.Mapping.Tasks
{
    public partial class ScheduleTaskMap : SeagullEntityTypeConfiguration<ScheduleTask>
    {
        public ScheduleTaskMap()
        {
            this.ToTable("ScheduleTask");
            this.HasKey(t => t.Id);
            this.Property(t => t.Name).IsRequired();
            this.Property(t => t.Type).IsRequired();
        }
    }
}