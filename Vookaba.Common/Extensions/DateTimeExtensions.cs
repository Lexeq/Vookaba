using System;
using System.Collections.Generic;

namespace Vookaba.Common.Extensions
{
    public static class DateTimeExtensions
    {
        private enum State { Begin, Number, Literal }

        private readonly static IReadOnlyDictionary<string, Func<DateTime, int, DateTime>> acts = new Dictionary<string, Func<DateTime, int, DateTime>>()
        {
            ["y"] = (dt, val) => dt.AddYears(val),
            ["d"] = (dt, val) => dt.AddDays(val),
            ["h"] = (dt, val) => dt.AddHours(val),
            ["m"] = (dt, val) => dt.AddMinutes(val),
        };

        public static bool TryAddFromString(this DateTime dateTime, string range, out DateTime result)
        {
            var state = State.Begin;
            var num = 0;
            result = dateTime;

            for (int i = 0; i < range.Length;)
            {
                switch (state)
                {
                    case State.Begin:
                        if (range[i] == ' ')
                        {
                            i++;
                            continue;
                        }
                        if (!char.IsDigit(range[i]))
                        {
                            return false;
                        }
                        state = State.Number;
                        break;
                    case State.Number:
                        if (char.IsDigit(range[i]))
                        {
                            num = num * 10 + (range[i] - '0');
                            i++;
                        }
                        else
                        {
                            state = State.Literal;
                        }
                        break;
                    case State.Literal:
                        //TODO: support literals with lenght > 1
                        var literal = range[i].ToString();
                        if (acts.ContainsKey(literal))
                        {
                            result = acts[literal].Invoke(result, num);
                            i++;
                            num = 0;
                            state = State.Begin;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    default:
                        throw new ApplicationException("Invalid state.");
                }
            }
            return true;
        }

        public static long GetUnixEpochOffset(this DateTime dateTime)
        {
            return (long)(dateTime - DateTime.UnixEpoch).TotalMilliseconds;
        }
    }
}
