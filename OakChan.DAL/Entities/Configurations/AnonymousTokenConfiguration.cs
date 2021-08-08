using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OakChan.Identity;

namespace OakChan.DAL.Entities.Configurations
{
    public class AnonymousTokenConfiguration : IEntityTypeConfiguration<AnonymousToken>
    {
        public void Configure(EntityTypeBuilder<AnonymousToken> builder)
        {
            builder.HasKey(t => t.Token);

            builder.HasIndex(t => t.UserId)
                .IsUnique(true);

            builder.Property(t => t.Created)
                .ValueGeneratedOnUpdateSometimes()
                .IsRequired(true);

            builder.Property(t => t.UserId)
                .IsRequired(false);

            builder.HasOne<ApplicationUser>()
                .WithOne()
                .IsRequired(false)
                .HasForeignKey<AnonymousToken>(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
