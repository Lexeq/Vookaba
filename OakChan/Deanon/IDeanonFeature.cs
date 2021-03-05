using System;
using System.Net;

namespace OakChan.Deanon
{
    public interface IDeanonFeature
    {
        public Guid UserToken { get; }

        public IPAddress IPAddress { get; }

        public string UserAgent { get; }
    }
}
