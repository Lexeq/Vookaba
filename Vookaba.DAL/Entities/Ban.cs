#nullable enable
using System;
using System.Net;
using Vookaba.DAL.Entities.Base;
using Vookaba.Identity;

namespace Vookaba.DAL.Entities
{
    public class Ban : ICreatedByUser, IHasCreationTime
    {
        public int Id { get; set; }

        public ApplicationUser? Creator { get; set; }

        public int UserId { get; set; }

        public DateTime Created { get; set; }

        public (IPAddress, int)? BannedNetwork { get; set; }

        public DateTime Expired { get; set; }

        public string? Reason { get; set; }

        public AuthorToken? BannedAuthor { get; set; }

        public Guid? BannedAothorToken { get; set; }

        public Board? Board { get; set; }

        public string? BoardKey { get; set; }

        public bool IsCanceled { get; set; }

        public int PostId { get; set; }
    }
}
