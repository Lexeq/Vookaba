#nullable enable
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vookaba.Common;
using Vookaba.DAL.Database;
using Vookaba.DAL.Entities;
using Vookaba.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vookaba.Services.DbServices
{
    public class DbModLogService : IModLogService
    {
        private readonly VookabaDbContext context;
        private readonly ILogger<DbModLogService> logger;
        private readonly IMapper mapper;

        public DbModLogService(VookabaDbContext context, IMapper mapper, ILogger<DbModLogService> logger)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<ModLogDto>> GetLogsForUserAsync(int userId, int offset, int count, bool lastFirst = true)
        {
            var query = context.ModActions.Where(m => m.UserId == userId);
            query = lastFirst ? query.OrderByDescending(m => m.Created) : query.OrderBy(m => m.Created);
            query = query.Skip(offset).Take(count);
            return await mapper.ProjectTo<ModLogDto>(query)
                .ToListAsync();
        }

        public async Task LogAsync(ApplicationEvent eventId, string entityId, string? note)
        {
            context.ModActions.Add(new ModAction
            {
                EventId = eventId,
                EntityId = entityId,
                Note = note
            });
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Create modlog failed.");
            }
        }
    }
}
