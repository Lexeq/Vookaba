#nullable enable
using Microsoft.Extensions.Logging;
using OakChan.Common;
using OakChan.DAL.Database;
using OakChan.DAL.Entities;
using System;
using System.Threading.Tasks;

namespace OakChan.Services.DbServices
{
    public class DbModLogService : IModLogService
    {
        private readonly OakDbContext context;
        private readonly ILogger<DbModLogService> logger;

        public DbModLogService(OakDbContext context, ILogger<DbModLogService> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public Task LogAsync(ApplicationEvent eventId, string entityId)
            => LogAsync(eventId, entityId, null);

        public async Task LogAsync(ApplicationEvent eventId, string entityId, string? message)
        {
            context.ModActions.Add(new ModAction
            {
                EventId = eventId,
                EntityId = entityId,
                Note = message
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
