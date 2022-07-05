using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Vookaba.Common;
using Vookaba.DAL.Entities;
using Vookaba.DAL.Entities.Base;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Vookaba.DAL.Database
{
    public class VookabaDbContext : IdentityDbContext
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public VookabaDbContext() { }

        public VookabaDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            this.httpContextAccessor = httpContextAccessor;
        }


        public virtual DbSet<Post> Posts { get; set; }

        public virtual DbSet<Thread> Threads { get; set; }

        public virtual DbSet<Board> Boards { get; set; }

        public virtual DbSet<Image> Images { get; set; }

        public virtual DbSet<ModAction> ModActions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(VookabaDbContext)));
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
                if (entry.Entity is ICreatedByAnonymous entAnon && entAnon.AuthorToken == default)
                {
                    entAnon.AuthorToken = Guid.Parse(httpContextAccessor.HttpContext.User.FindFirst(ApplicationConstants.ClaimTypes.AuthorToken).Value);
                }
                if (entry.Entity is ICreatedByUser entUser && entUser.UserId == default)
                {
                    entUser.UserId = int.Parse(httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                if (entry.Entity is IHasClientInfo entClient)
                {
                    entClient.IP ??= httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
                    entClient.UserAgent ??= httpContextAccessor.HttpContext.Request.Headers["User-Agent"];
                }
                if (entry.Entity is IHasCreationTime ct && ct.Created == default)
                {
                    ct.Created = DateTime.UtcNow;
                }
            }
        }
    }
}
