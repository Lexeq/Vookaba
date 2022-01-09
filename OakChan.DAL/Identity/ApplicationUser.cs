using Microsoft.AspNetCore.Identity;
using OakChan.DAL.Entities;
using System;

namespace OakChan.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public AuthorToken AuthorToken { get; set; }

        public Guid AuthorTokenId { get; set; }

    }
}
