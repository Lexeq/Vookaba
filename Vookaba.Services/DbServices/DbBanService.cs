using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using Vookaba.Common;
using Vookaba.Common.Exceptions;
using Vookaba.DAL.Database;
using Vookaba.DAL.Entities;
using Vookaba.Services.Abstractions;
using Vookaba.Services.Logging;

namespace Vookaba.Services.DbServices
{
    public class DbBanService : IBanService
    {
        private readonly VookabaDbContext context;
        private readonly IMapper mapper;
        private readonly IModLogService modLogs;
        private readonly ILogger<DbBanService> logger;

        private static (IPAddress Address, int Subnet) GetLowest((IPAddress Address, int Subnet) net)
        {
            var addressBytes = net.Address.GetAddressBytes();
            var bytes = net.Subnet;
            for (int i = 0; i < addressBytes.Length; i++)
            {
                var shift = bytes > 8 ? 8 : bytes > 0 ? bytes : 0;
                addressBytes[i] &= (byte)(0b11111111_00000000 >> shift);
                bytes -= 8;
            }
            return (Address: new IPAddress(addressBytes), Subnet: net.Subnet);
        }

        public DbBanService(VookabaDbContext context, IMapper mapper, IModLogService modLogs, ILogger<DbBanService> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.modLogs = modLogs;
            this.logger = logger;
        }

        public async Task CreateAsync(BanCreationDto ban)
        {
            ThrowHelper.ThrowIfNull(ban, nameof(ban));
            if (ban.BannedNetwork != null)
            {
                ban.BannedNetwork = GetLowest(ban.BannedNetwork.Value);
            }
            var banEntity = mapper.Map<Ban>(ban);

            context.Bans.Add(banEntity);
            await context.SaveChangesAsync();
            logger.BanCreated(banEntity);

            await modLogs.LogAsync(ApplicationEvent.BanCreated, banEntity.Id.ToString());
        }

        public async Task RemoveAsync(int banId)
        {
            var ban = context.Bans.Attach(new Ban
            {
                Id = banId,
                IsCanceled = true
            });
            ban.Property(b => b.IsCanceled).IsModified = true;

            await context.SaveChangesAsync();
            logger.BanRemoved(ban.Entity);

            await modLogs.LogAsync(ApplicationEvent.BanRemoved, banId.ToString());
        }

        public async Task<BanDto?> GetAsync(int banId)
        {
            var ban = await context.Bans.FindAsync(banId);
            return mapper.Map<BanDto?>(ban);
        }

        public async Task<BanInfoDto?> FindActiveBan(IPAddress address, Guid authorToken, string boardKey)
        {
            ThrowHelper.ThrowIfNull(address, nameof(address));
            ThrowHelper.ThrowIfNull(boardKey, nameof(boardKey));
            var ban = await context.Bans
                .Where(b => !b.IsCanceled && b.Expired > DateTime.UtcNow && (b.BoardKey == boardKey || b.BoardKey == null))
                .Where(b => b.BannedAothorToken == authorToken ||
                    EF.Functions.ContainedByOrEqual(address, b.BannedNetwork!.Value))
                .ProjectTo<BanInfoDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return ban;
        }
    }
}
