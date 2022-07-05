using Microsoft.AspNetCore.Identity;
using System;

namespace Vookaba.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public AuthorToken AuthorToken { get; set; }

        public Guid AuthorTokenId { get; set; }

    }
}
