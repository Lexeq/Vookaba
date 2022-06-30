using System;
using System.Reflection;

namespace Vookaba.Common
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
