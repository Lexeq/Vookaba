using System;
using System.Runtime.Serialization;

namespace OakChan.Services
{
    [Serializable]
    public class KeyNotFoundException : Exception
    {
        public KeyNotFoundException() : base() { }

        public KeyNotFoundException(string message) : base(message) { }

        public KeyNotFoundException(string message, Exception innerException) : base(message, innerException) { }

        protected KeyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
