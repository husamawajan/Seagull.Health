using Seagull.Core.Domain.Common;

namespace Seagull.Data.Mapping.Common
{
    public partial class AddressMap : SeagullEntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            this.ToTable("Address");
            this.HasKey(a => a.Id);

            this.HasOptional(a => a.Country)
                .WithMany()
                .HasForeignKey(a => a.CountryId).WillCascadeOnDelete(false);

            this.HasOptional(a => a.StateProvince)
                .WithMany()
                .HasForeignKey(a => a.StateProvinceId).WillCascadeOnDelete(false);
        }
    }
}
