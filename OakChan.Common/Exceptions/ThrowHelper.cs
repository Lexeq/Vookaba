using System;

namespace OakChan.Common.Exceptions
{
    public class ThrowHelper
    {
        public string ThrowIfNullOrWhiteSpace(string value, string name)
        {
            return ThrowIfNullOrWhiteSpace(value, name, $"{name} must not be an empty string.");
        }

        public string ThrowIfNullOrWhiteSpace(string value, string name, string exceptionMessage)
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

        public T ThrowIfNull<T>(T obj, string name) where T : class
        {
            return ThrowIfNull(obj, name, null);
        }

        public T ThrowIfNull<T>(T obj, string name, string exceptionMessage) where T : class
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
    }
}
