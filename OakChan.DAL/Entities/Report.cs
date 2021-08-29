using OakChan.DAL.Entities.Base;
using OakChan.Identity;
using System;
using System.Net;

namespace OakChan.DAL.Entities
{
    public class Report : IHasCreationTime, IHasAuthor
    {
        public int Id { get; set; }

        public bool IsProcessed { get; set; }

        public ApplicationUser ProcessedBy { get; set; }

        public string Reason { get; set; }

        public int PostId { get; set; }

        public Post Post { get; set; }

        public DateTime Created { get; set; }

        public Guid AuthorToken { get; set; }

        public IPAddress IP { get; set; }

        public string UserAgent { get; set; }
    }
}
