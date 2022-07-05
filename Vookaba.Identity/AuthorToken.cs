using System;
using System.Net;

namespace Vookaba.Identity
{
    public class AuthorToken
    {
        public Guid Token { get; set; }

        public IPAddress IP { get; set; }

        public DateTime Created { get; set; }
    }
}
