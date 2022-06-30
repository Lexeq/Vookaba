using System;

namespace Vookaba.Utils
{
    public static class DateTimeExtensions
    {
        public static long GetUnixEpochOffset(this DateTime dateTime)
        {
            return (long)(dateTime - DateTime.UnixEpoch).TotalMilliseconds;
        }
    }
}
