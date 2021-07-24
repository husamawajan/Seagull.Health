using Seagull.Core.Domain.Emails;
namespace Seagull.Data.Mapping.Emails
{
    public partial class EmailMap : SeagullEntityTypeConfiguration<Email>
    {
        public EmailMap()
        {
            this.ToTable("Email");
        }
    }
}