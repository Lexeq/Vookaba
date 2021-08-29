using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OakChan.Identity;

namespace OakChan.DAL.Entities.Configurations
{
    class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasOne<AuthorToken>()
                .WithOne()
                .HasForeignKey<ApplicationUser>(u => u.AuthorToken);
        }
    }
}
