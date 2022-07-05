using System;

namespace Vookaba.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static long GetUnixEpochOffset(this DateTime dateTime)
        {
            return (long)(dateTime - DateTime.UnixEpoch).TotalMilliseconds;
        }
    }
}
