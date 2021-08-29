using OakChan.DAL.Entities.Base;
using System;
using System.Net;

namespace OakChan.DAL.Entities
{
    public class AuthorToken : IHasCreationTime
    {
        public Guid Token { get; set; }

        public IPAddress IP { get; set; }

        public DateTime Created { get; set; }
    }
}
