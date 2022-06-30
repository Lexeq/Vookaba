using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vookaba.Common;

namespace Vookaba.DAL.Entities.Configurations
{
    public class BoardConfiguration : IEntityTypeConfiguration<Board>
    {
        public void Configure(EntityTypeBuilder<Board> builder)
        {
            builder.HasKey(b => b.Key);

            builder.Property(b => b.Key)
                .HasMaxLength(ApplicationConstants.BoardConstants.MaxKeyLength)
                .IsRequired();

            builder.Property(b => b.Name)
                .HasMaxLength(ApplicationConstants.BoardConstants.MaxNameLength)
                .IsRequired();

            builder.Property(b => b.IsHidden)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(b => b.IsDisabled)
                .HasDefaultValue(false)
                .IsRequired();

            builder.Property(b => b.BumpLimit)
                .HasDefaultValue(ApplicationConstants.BoardConstants.DefaultBumpLimit)
                .IsRequired();
        }
    }
}
