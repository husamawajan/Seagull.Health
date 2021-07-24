using Seagull.Core.Domain.Users;

namespace Seagull.Data.Mapping.Users
{
    public partial class RewardPointsHistoryMap : SeagullEntityTypeConfiguration<RewardPointsHistory>
    {
        public RewardPointsHistoryMap()
        {
            this.ToTable("RewardPointsHistory");
            this.HasKey(rph => rph.Id);

            this.Property(rph => rph.UsedAmount).HasPrecision(18, 4);

            this.HasRequired(rph => rph.User)
                .WithMany()
                .HasForeignKey(rph => rph.UserId);

        }
    }
}