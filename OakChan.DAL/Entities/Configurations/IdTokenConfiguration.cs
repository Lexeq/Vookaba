using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OakChan.DAL.Entities.Configurations
{
    public class IdTokenConfiguration : IEntityTypeConfiguration<IdToken>
    {
        public void Configure(EntityTypeBuilder<IdToken> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Created).IsRequired();

            builder.Property(a => a.IP).IsRequired();
        }
    }
}
