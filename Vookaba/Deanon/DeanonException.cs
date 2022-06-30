using System;

namespace Vookaba.Deanon
{
    [Serializable]
    public class DeanonException : Exception
    {
        public DeanonException() : base() { }
        public DeanonException(string message) : base(message) { }
        public DeanonException(string message, Exception inner) : base(message, inner) { }
        protected DeanonException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
