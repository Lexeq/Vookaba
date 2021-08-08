using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OakChan.DAL.Entities;
using OakChan.Identity;
using System.Reflection;

namespace OakChan.DAL.Database
{
    public class OakDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>
    {
        public OakDbContext() { }

        public OakDbContext(DbContextOptions options)
            : base(options) { }

        public DbSet<AnonymousToken> AnonymousTokens { get; set; }

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
    }
}
