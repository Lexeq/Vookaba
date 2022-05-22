using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OakChan.Common
{
    public static class EnumExtensions
    {
        public static bool IsValidFlagCombination(this Enum enumValue)
        {
            if (!enumValue.GetType().IsDefined(typeof(FlagsAttribute)))
            {
                throw new ArgumentException("Enum must be flags enum.");
            }
            var ch = enumValue.ToString()[0];
            return !(char.IsDigit(ch) || ch == '-');
        }
    }
}
