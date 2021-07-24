using Seagull.Core.Domain.Common;

namespace Seagull.Data.Mapping.Common
{
    public partial class AddressAttributeValueMap : SeagullEntityTypeConfiguration<AddressAttributeValue>
    {
        public AddressAttributeValueMap()
        {
            this.ToTable("AddressAttributeValue");
            this.HasKey(aav => aav.Id);
            this.Property(aav => aav.Name).IsRequired().HasMaxLength(400);

            this.HasRequired(aav => aav.AddressAttribute)
                .WithMany(aa => aa.AddressAttributeValues)
                .HasForeignKey(aav => aav.AddressAttributeId);
        }
    }
}