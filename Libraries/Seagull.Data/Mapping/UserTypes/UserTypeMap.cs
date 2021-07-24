using Seagull.Core.Domain.UserTypes;

namespace Seagull.Data.Mapping.UserTypes
{
	public partial class UserTypeMap : SeagullEntityTypeConfiguration<UserType>
	{
		public UserTypeMap()
		{
			this.ToTable("UserType");
			this.HasKey(c => c.Id);
		}
	}
}
	
 
