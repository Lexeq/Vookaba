using OakChan.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Net;

namespace OakChan.DAL.Entities
{
    public class Post : EntityWithCreationTime
    {
        public int Id { get; set; }

        public int ThreadId { get; set; }

        public Thread Thread { get; set; }

        public int Number { get; set; }

        public List<Attachment> Attachments{ get; set; }

        public bool IsOP { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }

        public bool IsSaged { get; set; }

        public Guid AnonymousToken { get; set; }

        public IPAddress AuthorIP { get; set; }

        public string AuthorUserAgent { get; set; }
    }
}
