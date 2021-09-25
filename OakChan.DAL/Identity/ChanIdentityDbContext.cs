﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace OakChan.Identity
{
    public class ChanIdentityDbContext<TUser, TRole, TInvitation, TKey> : IdentityDbContext<TUser, TRole, TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TInvitation : Invitation<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        public ChanIdentityDbContext() { }

        public ChanIdentityDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<TInvitation> Invitations { get; set; }


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
        }
    }
}