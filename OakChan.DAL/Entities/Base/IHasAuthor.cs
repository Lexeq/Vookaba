using System;
using System.Net;

namespace OakChan.DAL.Entities.Base
{
    interface IHasAuthor
    {
        public Guid AuthorToken { get; set; }

        public IPAddress IP { get; set; }

        public string UserAgent { get; set; }
    }
}
