using Seagull.Core.Domain.Users;

namespace Seagull.Data.Mapping.Users
{
    public partial class ExternalAuthenticationRecordMap : SeagullEntityTypeConfiguration<ExternalAuthenticationRecord>
    {
        public ExternalAuthenticationRecordMap()
        {
            this.ToTable("ExternalAuthenticationRecord");

            this.HasKey(ear => ear.Id);

            this.HasRequired(ear => ear.User)
                .WithMany(c => c.ExternalAuthenticationRecords)
                .HasForeignKey(ear => ear.UserId);

        }
    }
}