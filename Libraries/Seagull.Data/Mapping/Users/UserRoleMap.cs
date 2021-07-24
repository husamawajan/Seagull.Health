using Seagull.Core.Domain.Users;

namespace Seagull.Data.Mapping.Users
{
    public partial class UserRoleMap : SeagullEntityTypeConfiguration<UserRole>
    {
        public UserRoleMap()
        {
            this.ToTable("UserRole");
            this.HasKey(cr => cr.Id);
            this.Property(cr => cr.Name).IsRequired().HasMaxLength(255);
            this.Property(cr => cr.SystemName).HasMaxLength(255);
        }
    }
}