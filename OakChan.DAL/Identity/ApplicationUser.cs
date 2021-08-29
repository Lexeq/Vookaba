using Microsoft.AspNetCore.Identity;
using System;

namespace OakChan.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public Guid AuthorToken { get; set; }

    }
}
