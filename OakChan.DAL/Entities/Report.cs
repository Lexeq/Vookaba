using OakChan.DAL.Entities.Base;
using OakChan.Identity;
using System;
using System.Net;

namespace OakChan.DAL.Entities
{
    public class Report : EntityWithCreationTime
    {
        public int Id { get; set; }

        public IPAddress ComplainantIP { get; set; }

        public string ComplainantUserAgent { get; set; }

        public Guid AnonymousToken { get; set; }

        public bool IsProcessed { get; set; }

        public ApplicationUser ProcessedBy { get; set; }

        public string Reason { get; set; }

        public int PostId { get; set; }

        public Post Post { get; set; }

    }
}
