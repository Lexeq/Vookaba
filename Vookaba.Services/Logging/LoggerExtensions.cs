using Microsoft.Extensions.Logging;
using Vookaba.Common;
using Vookaba.DAL.Entities;

namespace Vookaba.Services.Logging
{
    internal static class LoggerExtensions
    {
        private static readonly Action<ILogger, string, string, Exception?> _banCreated;
        private static readonly Action<ILogger, int, Exception?> _banRemoved;
        private static readonly Action<ILogger, int, Exception?> _modLogRecordAdded;
        private static readonly Action<ILogger, Exception?> _modLogRecordFailed;
        static LoggerExtensions()
        {
            _banCreated = LoggerMessage.Define<string, string>(LogLevel.Debug, (int)ApplicationEvent.BanCreated, "Ban created for IP: {IP} and token: {Token}.");
            _banRemoved = LoggerMessage.Define<int>(LogLevel.Debug, (int)ApplicationEvent.BanRemoved, "Ban with Id = {Id} removed.");
            _modLogRecordAdded = LoggerMessage.Define<int>(LogLevel.Debug, (int)ApplicationEvent.ModLogCreated, "ModLog record created with Id = {Id}.");
            _modLogRecordFailed = LoggerMessage.Define(LogLevel.Error, (int)ApplicationEvent.ModLogCreatingFailed, "Can't create modLog record.");
        }
        public static void BanCreated(this ILogger logger, Ban ban)
        {
            _banCreated(logger, ban.BannedNetwork?.ToString() ?? "none", ban.BannedAothorToken?.ToString() ?? "none", null);
        }

        public static void BanRemoved(this ILogger logger, Ban ban)
        {
            _banRemoved(logger, ban.Id, null);
        }

        public static void ModLogAdded(this ILogger logger, int id)
        {
            _modLogRecordAdded(logger, id, null);
        }

        public static void ModLogCreatingFailed(this ILogger logger, Exception exception)
        {
            _modLogRecordFailed(logger, exception);
        }
    }
}
