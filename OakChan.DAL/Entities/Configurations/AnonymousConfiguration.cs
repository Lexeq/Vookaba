using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OakChan.DAL.Entities.Configurations
{
    public class AnonymousConfiguration : IEntityTypeConfiguration<Anonymous>
    {
        public void Configure(EntityTypeBuilder<Anonymous> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Created).IsRequired();

            builder.Property(a => a.IP).IsRequired();
        }
    }
}
