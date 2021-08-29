using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OakChan.Identity;

namespace OakChan.DAL.Entities.Configurations
{
    public class AuthorTokenConfiguration : IEntityTypeConfiguration<AuthorToken>
    {
        public void Configure(EntityTypeBuilder<AuthorToken> builder)
        {
            builder.HasKey(t => t.Token);

            builder.Property(t => t.Created)
                .IsRequired(true);
        }
    }
}
