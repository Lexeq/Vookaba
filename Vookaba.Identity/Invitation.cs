using Microsoft.AspNetCore.Identity;
using System;

namespace Vookaba.Identity
{
    public class Invitation<TKey> where TKey : struct, IEquatable<TKey>
    {
        public Guid Id { get; set; }

        public TKey PublisherId { get; set; }

        public DateTime Created { get; set; }

        public DateTime Expire { get; set; }

        public bool IsUsed { get; set; }

        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    }

}
