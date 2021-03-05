using System;
using System.Net;

namespace OakChan.Deanon
{
    public class DeanonFeature : IDeanonFeature
    {
        public Guid UserToken { get; set; }

        public IPAddress IPAddress { get; set; }

        public string UserAgent { get; set; }
    }
}
