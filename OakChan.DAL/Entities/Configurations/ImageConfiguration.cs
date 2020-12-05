using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OakChan.DAL.Entities.Configurations
{
    public class ImageConfiguration : IEntityTypeConfiguration<Image>
    {
        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.OriginalName).IsRequired();

            builder.Property(i => i.UploadDate).IsRequired();

            builder.Property(i => i.Hash).IsRequired();

            builder.Property(i => i.Type).IsRequired();
        }
    }
}
