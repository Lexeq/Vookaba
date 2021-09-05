using Microsoft.AspNetCore.Identity;
using System;

namespace OakChan.Identity
{
    public class Invitation<TKey> where TKey : struct, IEquatable<TKey> 
    {
        public Guid Id { get; set; }

        public TKey PublisherId { get; set; }

        public IdentityUser<TKey> Publisher { get; set; }

        public DateTime Created { get; set; }

        public DateTime Expire { get; set; }

        public TKey? UsedByID { get; set; }

        public IdentityUser<TKey> UsedBy { get; set; }

        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    }

}
