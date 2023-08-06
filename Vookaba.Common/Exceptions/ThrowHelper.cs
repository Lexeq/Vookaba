using System;
using System.Reflection;
using Vookaba.Common.Extensions;

namespace Vookaba.Common.Exceptions
{
    public static class ThrowHelper
    {
        public static string ThrowIfNullOrWhiteSpace(string value, string name)
        {
            return ThrowIfNullOrWhiteSpace(value, name, $"{name} must not be an empty string.");
        }

        public static string ThrowIfNullOrWhiteSpace(string value, string name, string exceptionMessage)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name, exceptionMessage);
            }
            else if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(exceptionMessage, name);
            }
            return value;
        }

        public static T ThrowIfNull<T>(T obj, string name) where T : class
        {
            return ThrowIfNull(obj, name, null);
        }

        public static T ThrowIfNull<T>(T obj, string name, string exceptionMessage) where T : class
        {
            if (obj == null)
            {
                if (exceptionMessage == null)
                {
                    throw new ArgumentNullException(name);
                }
                else
                {
                    throw new ArgumentNullException(name, exceptionMessage);
                }
            }
            return obj;
        }

        public static T ThrowIfEnumIsNotCorrect<T>(T value) where T : struct, Enum
        {
            if (typeof(T).IsDefined(typeof(FlagsAttribute)))
            {
                if (!value.IsValidFlagCombination())
                {
                    throw new ArgumentException("Invalid combination of flags.");
                }
            }
            else if (!Enum.IsDefined(value))
            {
                throw new ArgumentException($"'{value}' is not defined in {typeof(T).Name}.");
            }
            return value;
        }
    }
}
