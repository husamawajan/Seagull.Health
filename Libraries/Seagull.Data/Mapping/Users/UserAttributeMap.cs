using Seagull.Core.Domain.Users;

namespace Seagull.Data.Mapping.Users
{
    public partial class UserAttributeMap : SeagullEntityTypeConfiguration<UserAttribute>
    {
        public UserAttributeMap()
        {
            this.ToTable("UserAttribute");
            this.HasKey(ca => ca.Id);
            this.Property(ca => ca.Name).IsRequired().HasMaxLength(400);

        }
    }
}