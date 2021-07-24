using Seagull.Core.Domain.Users;

namespace Seagull.Data.Mapping.Users
{
    public partial class UserAttributeValueMap : SeagullEntityTypeConfiguration<UserAttributeValue>
    {
        public UserAttributeValueMap()
        {
            this.ToTable("UserAttributeValue");
            this.HasKey(cav => cav.Id);
            this.Property(cav => cav.Name).IsRequired().HasMaxLength(400);

            this.HasRequired(cav => cav.UserAttribute)
                .WithMany(ca => ca.UserAttributeValues)
                .HasForeignKey(cav => cav.UserAttributeId);
        }
    }
}