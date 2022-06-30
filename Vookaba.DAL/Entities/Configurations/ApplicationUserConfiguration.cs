using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vookaba.Identity;

namespace Vookaba.DAL.Entities.Configurations
{
    class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.AuthorTokenId)
                .HasColumnName("AuthorToken");

            builder.HasOne(u => u.AuthorToken)
                .WithOne()
                .HasForeignKey<ApplicationUser>(u => u.AuthorTokenId);
        }
    }
}
