using OakChan.DAL.Entities.Base;
using System;
using System.Collections.Generic;
using System.Net;

namespace OakChan.DAL.Entities
{
    public class Post : IHasCreationTime, ICreatedByAnonymous, IHasClientInfo
    {
        public int Id { get; set; }

        public int ThreadId { get; set; }

        public Thread Thread { get; set; }

        public int Number { get; set; }

        public List<Attachment> Attachments { get; set; }

        public bool IsOP { get; set; }

        public bool OpMark { get; set; }

        public string Name { get; set; }

        public string HtmlEncodedMessage { get; set; }

        public string PlainMessageText { get; set; }

        public bool IsSaged { get; set; }

        public DateTime Created { get; set; }

        public Guid AuthorToken { get; set; }

        public IPAddress IP { get; set; }

        public string UserAgent { get; set; }

        public string Tripcode { get; set; }

    }
}
