using Seagull.Core.Domain.Common;

namespace Seagull.Data.Mapping.Common
{
    public partial class AddressAttributeMap : SeagullEntityTypeConfiguration<AddressAttribute>
    {
        public AddressAttributeMap()
        {
            this.ToTable("AddressAttribute");
            this.HasKey(aa => aa.Id);
            this.Property(aa => aa.Name).IsRequired().HasMaxLength(400);
        }
    }
}