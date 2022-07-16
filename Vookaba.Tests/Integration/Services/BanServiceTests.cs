using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Vookaba.DAL.Entities;
using Vookaba.Services.Abstractions;
using Vookaba.Services.DbServices;
using Vookaba.Services.DTO;
using Vookaba.Tests.Integration.Base;

namespace Vookaba.Tests.Integration.Services
{
    public class BanServiceTests : ServiceTestsBase
    {

        public IBanService GetBanService()
        {
            return new DbBanService(GetDbContext(), ServiceDtoMapper, Mock.Of<IModLogService>(), Mock.Of<ILogger<DbBanService>>());
        }

        [TestCase("142.25.0.0", 16)]
        [TestCase("142.25.98.145", 8)]
        [TestCase("142.25.98.145", 32)]
        public async Task CreateBan(string ipString, int subnet)
        {
            SeedData.AddDefaults().AddUsers(new Identity.ApplicationUser
            {
                Id = 1,
                AuthorTokenId = new Guid("11111111-1111-1111-1111-111111111111")
            });
            var newBan = new BanCreationDto
            {
                BannedNetwork = (IPAddress.Parse("130.12.0.11"), 26),
                Board = "b",
                Expired = DateTime.UtcNow.AddDays(100),
                Reason = "test_ban"
            };

            var service = GetBanService();
            await service.CreateAsync(newBan);

            var ban = await GetDbContext().Bans.FirstOrDefaultAsync();

            Assert.NotNull(ban);
            Assert.AreEqual(newBan.Reason, ban.Reason);
            Assert.IsNull(ban.BannedAothorToken);
        }

        [TestCaseSource(typeof(CaseSource), nameof(CaseSource.Cases))]
        public async Task FindBan(BanCase data)
        {
            SeedData.AddDefaults().AddTokens(new Identity.AuthorToken
            {
                Token = new Guid("22222222-2222-2222-2222-222222222222")
            }).AddUsers(new Identity.ApplicationUser
            {
                Id = 1,
                AuthorTokenId = new Guid("11111111-1111-1111-1111-111111111111")
            }).AddBans(data.Ban);

            var service = GetBanService();

            var ban = await service.FindActiveBan(data.Request.Address, data.Request.AuthorToken, data.Request.Board);

            Assert.AreEqual(data.IsBanned, ban is not null);
            Assert.AreEqual(data.IsBanned, ban?.Reason != null);
        }

        class CaseSource
        {
            public static IEnumerable<BanCase> Cases()
            {
                var bannedToken = new Guid("22222222-2222-2222-2222-222222222222");
                var bannedIp = IPAddress.Parse("10.10.10.100");
                var bannedOnBoard = "a";
                var request = (bannedIp, bannedToken, bannedOnBoard);

                //banned by ip
                yield return new BanCase
                {
                    Ban = new Ban
                    {
                        BannedNetwork = (IPAddress.Parse("10.10.10.0"), 24),
                        Reason = "test bans",
                        Created = DateTime.UtcNow.Subtract(TimeSpan.FromDays(100)),
                        Expired = DateTime.UtcNow.AddDays(100),
                        UserId = 1
                    },
                    Request = request,
                    IsBanned = true
                };

                //user ip out of banned network
                yield return new BanCase
                {
                    Ban = new Ban
                    {
                        BannedNetwork = (IPAddress.Parse("10.10.10.0"), 28),
                        Reason = "test bans",
                        Created = DateTime.UtcNow.Subtract(TimeSpan.FromDays(100)),
                        Expired = DateTime.UtcNow.AddDays(100),
                        UserId = 1
                    },
                    Request = request,
                    IsBanned = false
                };

                //ban token
                yield return new BanCase
                {
                    Ban = new Ban
                    {
                        BannedNetwork = null,
                        Reason = "ban",
                        BannedAothorToken = bannedToken,
                        Created = DateTime.UtcNow.Subtract(TimeSpan.FromDays(100)),
                        Expired = DateTime.UtcNow.AddDays(100),
                        UserId = 1
                    },
                    Request = request,
                    IsBanned = true
                };

                //expired ban
                yield return new BanCase
                {
                    Ban = new Ban
                    {
                        BannedNetwork = (IPAddress.Parse("10.0.0.0"), 8),
                        Reason = "ban",
                        BannedAothorToken = bannedToken,
                        Created = DateTime.UtcNow.AddDays(-100),
                        Expired = DateTime.UtcNow.AddDays(-10),
                        UserId = 1
                    },
                    Request = request,
                    IsBanned = false
                };

                //banned, but on other board
                yield return new BanCase
                {
                    Ban = new Ban
                    {
                        BannedNetwork = (IPAddress.Parse("10.0.0.0"), 8),
                        Reason = "ban",
                        BannedAothorToken = bannedToken,
                        Created = DateTime.UtcNow.AddDays(-100),
                        Expired = DateTime.UtcNow.AddDays(10),
                        UserId = 1,
                        BoardKey = "b"
                    },
                    Request = request,
                    IsBanned = false
                };

                //bad ban
                yield return new BanCase
                {
                    Ban = new Ban
                    {
                        Reason = "ban",
                        Created = DateTime.UtcNow.AddDays(-100),
                        Expired = DateTime.UtcNow.AddDays(10),
                        UserId = 1
                    },
                    Request = request,
                    IsBanned = false
                };
            }
        }

        public class BanCase
        {
            public Ban Ban { get; init; }

            public (IPAddress Address, Guid AuthorToken, string Board) Request { get; init; }

            public bool IsBanned { get; init; }
        }
    }
}
