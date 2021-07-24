using Seagull.Core.Domain.Security;

namespace Seagull.Data.Mapping.Security
{
    public partial class AclRecordMap : SeagullEntityTypeConfiguration<AclRecord>
    {
        public AclRecordMap()
        {
            this.ToTable("AclRecord");
            this.HasKey(ar => ar.Id);

            this.Property(ar => ar.EntityName).IsRequired().HasMaxLength(400);

            this.HasRequired(ar => ar.UserRole)
                .WithMany()
                .HasForeignKey(ar => ar.UserRoleId)
                .WillCascadeOnDelete(true);
        }
    }
}