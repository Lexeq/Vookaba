using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OakChan.Identity;
using System;

namespace OakChan.DAL.Database
{
    public class IdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, ApplicationInvitation, AuthorToken, int>
    {
        public IdentityDbContext() { }

        public IdentityDbContext(DbContextOptions options) : base(options) { }
    }

    public class IdentityDbContext<TUser, TRole, TInvitation, TToken, TKey> : IdentityDbContext<TUser, TRole, TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TInvitation : Invitation<TKey>
        where TToken : AuthorToken
        where TKey : struct, IEquatable<TKey>
    {
        public IdentityDbContext() { }

        public IdentityDbContext(DbContextOptions options) : base(options)
        { }

        public virtual DbSet<TToken> AuthorTokens { get; set; }

        public virtual DbSet<TInvitation> Invitations { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity(delegate (EntityTypeBuilder<TInvitation> b)
            {
                b.HasKey(i => i.Id);

                b.Property(i => i.ConcurrencyStamp)
                    .IsConcurrencyToken()
                    .IsRequired();

                b.HasIndex(i => i.IsUsed)
                    .IsUnique(false);

                b.HasIndex(i => i.Expire);
            });

            builder.Entity(delegate (EntityTypeBuilder<AuthorToken> x)
            {
                x.HasKey(t => t.Token);

                x.Property(t => t.Created)
                    .IsRequired(true);
            });
        }
    }
}
