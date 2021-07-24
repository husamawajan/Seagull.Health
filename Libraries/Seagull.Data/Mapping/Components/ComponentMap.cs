using Seagull.Core.Domain.Components;

namespace Seagull.Data.Mapping.Components
{
	public partial class ComponentMap : SeagullEntityTypeConfiguration<Component>
	{
		public ComponentMap()
		{
			this.ToTable("Component");
			this.HasKey(c => c.Id);
		}
	}
}
