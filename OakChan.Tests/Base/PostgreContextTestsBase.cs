using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OakChan.Common;
using OakChan.DAL.Database;
using OakChan.Deanon;
using System.Linq;
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
                _outerContext = CreateNewContext();
            }
            return _outerContext;
        }


        [OneTimeSetUp]
        public void Initialize()
        {
            using var _context = CreateNewContext();
            _context.Database.EnsureDeleted();
            _context.Database.Migrate();
            TruncateTables(); //drop all default data
        }

        [OneTimeTearDown]
        public void Finish()
        {
            using var context = CreateNewContext();
            context.Database.EnsureDeleted();
        }


        [SetUp]
        public void SetUp()
        {
            _seeder = new Seeder(CreateNewContext);
        }

        [TearDown]
        public void Clear()
        {
            _outerContext = null;
            _seeder = null;
            TruncateTables();
            return;
        }



        private OakDbContext CreateNewContext()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Loopback;
            httpContext.Request.Headers["User-Agent"] = nameof(PostgreContextTestsBase);
            httpContext.User.Identities.First().AddClaim(new System.Security.Claims.Claim(OakConstants.AuthorTokenClaimType, "11111111-1111-1111-1111-111111111111"));
            var httpAM = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
            httpAM.Setup(m => m.HttpContext).Returns(httpContext);
            return new OakDbContext(_dbContextOptions, httpAM.Object);
        }

        //TODO: truncate ALL tables
        private void TruncateTables()
        {
            using var context = CreateNewContext();
            context.Database.ExecuteSqlRaw(@"TRUNCATE TABLE ""Boards"" RESTART IDENTITY CASCADE");
            context.Database.ExecuteSqlRaw(@"TRUNCATE TABLE ""Threads"" RESTART IDENTITY CASCADE");
            context.Database.ExecuteSqlRaw(@"TRUNCATE TABLE ""AuthorTokens"" RESTART IDENTITY CASCADE");
            context.Database.ExecuteSqlRaw(@"TRUNCATE TABLE ""Attachment"" RESTART IDENTITY CASCADE");
            context.Database.ExecuteSqlRaw(@"TRUNCATE TABLE ""Posts"" RESTART IDENTITY CASCADE");
        }
    }
}
