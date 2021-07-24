using Seagull.Core.Domain.Messages;

namespace Seagull.Data.Mapping.Messages
{
    public partial class CampaignMap : SeagullEntityTypeConfiguration<Campaign>
    {
        public CampaignMap()
        {
            this.ToTable("Campaign");
            this.HasKey(ea => ea.Id);

            this.Property(ea => ea.Name).IsRequired();
            this.Property(ea => ea.Subject).IsRequired();
            this.Property(ea => ea.Body).IsRequired();
        }
    }
}