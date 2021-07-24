using Seagull.Core.Domain.Users;

namespace Seagull.Data.Mapping.Users
{
    public partial class UserPasswordMap : SeagullEntityTypeConfiguration<UserPassword>
    {
        public UserPasswordMap()
        {
            this.ToTable("UserPassword");
            this.HasKey(password => password.Id);

            this.HasRequired(password => password.User)
                .WithMany()
                .HasForeignKey(password => password.UserId);

            this.Ignore(password => password.PasswordFormat);
        }
    }
}