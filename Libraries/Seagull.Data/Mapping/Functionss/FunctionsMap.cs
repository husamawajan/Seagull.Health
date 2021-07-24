using Seagull.Core.Domain.Functionss;

namespace Seagull.Data.Mapping.Functionss
{
	public partial class FunctionsMap : SeagullEntityTypeConfiguration<Functions>
	{
		public FunctionsMap()
		{
			this.ToTable("Functions");
			this.HasKey(c => c.Id);
		}
	}
}
	
 
