using Seagull.Core.Domain.Media;

namespace Seagull.Data.Mapping.Media
{
    public partial class DownloadMap : SeagullEntityTypeConfiguration<Download>
    {
        public DownloadMap()
        {
            this.ToTable("Download");
            this.HasKey(p => p.Id);
            this.Property(p => p.DownloadBinary).IsMaxLength();
        }
    }
}