using OakChan.DAL.Entities.Base;
using System;
using System.Net;

namespace OakChan.DAL.Entities
{
    public class AnonymousToken : EntityWithCreationTime
    {
        public Guid Token { get; set; }

        public int? UserId { get; set; }

        public IPAddress IP { get; set; }
    }
}
