using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vookaba.DAL.Entities.Base;

namespace Vookaba.DAL.Entities.Configurations
{
    public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.OriginalName)
                .IsRequired();

            builder.Property(i => i.Hash)
                .IsFixedLength()
                .IsRequired();

            builder.Property(i => i.Extension)
                .IsRequired();

            builder.Property(i => i.Name)
                .IsRequired();

            builder.HasOne(i => i.Post)
                .WithMany(p => p.Attachments)
                .HasForeignKey(i => i.PostId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasDiscriminator<int>("AttachmentType")
                .HasValue<Image>((int)AttachmentTypes.Image);

            builder.HasIndex(i => i.Hash);

            builder.HasIndex(i => new { i.PostId });
        }
    }
}
