using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OakChan.DAL.Database;
using System;
using System.Reflection;

namespace OakChan.Tests.Base
{
    public class PostgreContextTestsBase
    {
        private readonly DbContextOptions<OakDbContext> _dbContextOptions;
        private OakDbContext _outerContext;
        private Seeder _seeder;

        protected DbSeedData SeedData => _seeder.Data;

        public PostgreContextTestsBase()
        {
            var cfg = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .Build();

            _dbContextOptions = new DbContextOptionsBuilder<OakDbContext>()
                .UseNpgsql(cfg.GetConnectionString("TestsDb"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution)
#if DEBUG
                .UseLoggerFactory(LoggerFactory.Create(x =>
                {
                    x.ClearProviders();
                    x.AddDebug();
                }))
                .EnableSensitiveDataLogging()
#endif
                .Options;
        }

        protected OakDbContext GetDbContext()
        {
            if (!_seeder.Seeded)
            {
                _seeder.Seed();
            }
            if (_outerContext == null)
            {
                _outerContext = new OakDbContext(_dbContextOptions);
            }
            return _outerContext;
        }


        [OneTimeSetUp]
        public void Initialize()
        {
            using var _context = new OakDbContext(_dbContextOptions);
            _context.Database.EnsureDeleted();
            _context.Database.Migrate();
            TruncateTables(); //drop all default data
        }

        [OneTimeTearDown]
        public void Finish()
        {
            using var context = new OakDbContext(_dbContextOptions);
            context.Database.EnsureDeleted();
        }


        [SetUp]
        public void SetUp()
        {
            _seeder = new Seeder(_dbContextOptions);
        }

        [TearDown]
        public void Clear()
        {
            _outerContext = null;
            _seeder = null;
            TruncateTables();
            return;
        }

        //TODO: truncate ALL tables
        private void TruncateTables()
        {
            using var context = new OakDbContext(_dbContextOptions);
            context.Database.ExecuteSqlRaw(@"TRUNCATE TABLE ""Boards"" RESTART IDENTITY CASCADE");
            context.Database.ExecuteSqlRaw(@"TRUNCATE TABLE ""Threads"" RESTART IDENTITY CASCADE");
            context.Database.ExecuteSqlRaw(@"TRUNCATE TABLE ""AnonymousTokens"" RESTART IDENTITY CASCADE");
            context.Database.ExecuteSqlRaw(@"TRUNCATE TABLE ""Attachment"" RESTART IDENTITY CASCADE");
            context.Database.ExecuteSqlRaw(@"TRUNCATE TABLE ""Posts"" RESTART IDENTITY CASCADE");
        }
    }
}
