using Seagull.Core.Domain.Directory;

namespace Seagull.Data.Mapping.Directory
{
    public partial class CurrencyMap : SeagullEntityTypeConfiguration<Currency>
    {
        public CurrencyMap()
        {
            this.ToTable("Currency");
            this.HasKey(c =>c.Id);
            this.Property(c => c.Name).IsRequired().HasMaxLength(50);
            this.Property(c => c.CurrencyCode).IsRequired().HasMaxLength(5);
            this.Property(c => c.DisplayLocale).HasMaxLength(50);
            this.Property(c => c.CustomFormatting).HasMaxLength(50);
            this.Property(c => c.Rate).HasPrecision(18, 4);

            this.Ignore(c => c.RoundingType);
        }
    }
}