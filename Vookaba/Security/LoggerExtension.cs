using Microsoft.Extensions.Logging;
using System;
using Vookaba.Common;

namespace Vookaba.Security
{
    public static class LoggerExtensions
    {
        private static Action<ILogger, Exception> _noBoardValue;

        static LoggerExtensions()
        {
            _noBoardValue = LoggerMessage.Define(LogLevel.Warning, (int)ApplicationEvent.ApplicationProlem, "Route value for board was not provided.");
        }

        public static void NoBoardValue(this ILogger logger)
        {
            _noBoardValue(logger, null);
        }
    }
}
