using System;

namespace OakChan.Utils
{
    public static class DateTimeExtensions
    {
        public static long GetUnixEpochOffset(this DateTime dateTime)
        {
            return (long)(dateTime - DateTime.UnixEpoch).TotalMilliseconds;
        }
    }
}
