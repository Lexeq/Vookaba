using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OakChan.Common;
using OakChan.DAL.Entities;
using OakChan.DAL.Entities.Base;
using OakChan.Identity;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OakChan.DAL.Database
{
    public class OakDbContext : ChanIdentityDbContext<ApplicationUser, ApplicationRole, ApplicationInvitation, int>
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public OakDbContext() { }

        public OakDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public DbSet<AuthorToken> AuthorTokens { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Thread> Threads { get; set; }

        public DbSet<Board> Boards { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<BoardModerator> BoardModerators { get; set; }

        public DbSet<Report> Reports { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(OakDbContext)));
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AutoSetProperties();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, System.Threading.CancellationToken cancellationToken = default)
        {
            AutoSetProperties();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        private void AutoSetProperties()
        {
            ChangeTracker.DetectChanges();
            foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
            {
                if (entry.Entity is IHasAuthor ha)
                {
                    ha.IP ??= httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
                    ha.UserAgent ??= httpContextAccessor.HttpContext.Request.Headers["User-Agent"];
                    if (ha.AuthorToken == default)
                    {
                        ha.AuthorToken = Guid.Parse(httpContextAccessor.HttpContext.User.FindFirst(OakConstants.ClaimTypes.AuthorToken).Value);
                    }
                }
                if (entry.Entity is IHasCreationTime ct && ct.Created == default)
                {
                    ct.Created = DateTime.UtcNow;
                }
            }
        }
    }
}
