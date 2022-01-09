using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OakChan.Identity;

namespace OakChan.DAL.Entities.Configurations
{
    class ApplicationInvitationConfiguration : IEntityTypeConfiguration<ApplicationInvitation>
    {
        public void Configure(EntityTypeBuilder<ApplicationInvitation> builder)
        {
            builder.HasOne(i => i.Publisher)
                .WithMany()
                .HasForeignKey(i => i.PublisherId)
                .IsRequired(true);

            builder.HasOne(i => i.UsedBy)
                .WithMany()
                .HasForeignKey(i => i.UsedByID)
                .IsRequired(false);
        }
    }
}
