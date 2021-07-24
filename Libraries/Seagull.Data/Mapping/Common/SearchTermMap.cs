using Seagull.Core.Domain.Common;

namespace Seagull.Data.Mapping.Common
{
    public partial class SearchTermMap : SeagullEntityTypeConfiguration<SearchTerm>
    {
        public SearchTermMap()
        {
            this.ToTable("SearchTerm");
            this.HasKey(st => st.Id);
        }
    }
}
