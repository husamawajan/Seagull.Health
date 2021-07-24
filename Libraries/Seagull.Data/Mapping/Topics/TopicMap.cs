using Seagull.Core.Domain.Topics;

namespace Seagull.Data.Mapping.Topics
{
    public class TopicMap : SeagullEntityTypeConfiguration<Topic>
    {
        public TopicMap()
        {
            this.ToTable("Topic");
            this.HasKey(t => t.Id);
        }
    }
}
