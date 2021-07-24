using Seagull.Core.Domain.UserEntitys;

namespace Seagull.Data.Mapping.UserEntitys
{
	public partial class UserEntityMap : SeagullEntityTypeConfiguration<UserEntity>
	{
		public UserEntityMap()
		{
			this.ToTable("UserEntity");
			this.HasKey(c => c.Id);
            this.HasOptional(a => a.FK_UserType)
                .WithMany()
                .HasForeignKey(a => a.UserTypeId);
		}
	}
}
