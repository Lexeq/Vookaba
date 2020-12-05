using Microsoft.EntityFrameworkCore;
using OakChan.DAL.Entities;
using System.Reflection;

namespace OakChan.DAL.Database
{
    public class OakDbContext : DbContext
    {
        public OakDbContext() { }

        public OakDbContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Anonymous> Anonymous { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Thread> Threads { get; set; }

        public DbSet<Board> Boards { get; set; }

        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
