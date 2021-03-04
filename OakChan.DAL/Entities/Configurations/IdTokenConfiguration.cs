using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OakChan.Identity;

namespace OakChan.DAL.Entities.Configurations
{
    public class IdTokenConfiguration : IEntityTypeConfiguration<IdToken>
    {
        public void Configure(EntityTypeBuilder<IdToken> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Created).IsRequired();

            builder.HasIndex(t => t.UserId)
                .IsUnique(true);

            builder.HasOne<ApplicationUser>()
                .WithOne()
                .IsRequired(false)
                .HasForeignKey<IdToken>(j => j.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
