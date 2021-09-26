using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OakChan.DAL.Entities.Configurations
{
    public class ModActionConfiguration : IEntityTypeConfiguration<ModAction>
    {
        public void Configure(EntityTypeBuilder<ModAction> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Created)
                .IsRequired();

            builder.Property(m => m.IP)
                .IsRequired();

            builder.Property(m => m.UserAgent)
                .IsRequired();

            builder.Property(m => m.EventId)
                .IsRequired();

            builder.Property(m => m.Note)
                .HasMaxLength(1024)
                .IsRequired(false);

            builder.Property(m => m.EntityId)
                .IsRequired();

            builder.HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .IsRequired();

            builder.HasIndex(m => new { m.UserId, m.Created });
        }
    }
}
