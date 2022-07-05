using Microsoft.AspNetCore.Http;
using Vookaba.Markup;
using Vookaba.Markup.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using static Vookaba.Common.ApplicationConstants.Roles;

namespace Vookaba.Utils
{
    public class MarkupTagsFactory : IMarkupTagsFactory
    {
        private static readonly IMarkupTag[] publicTags;
        private static readonly IMarkupTag[] janitorTags;
        private static readonly IMarkupTag[] modTags;
        private static readonly IMarkupTag[] adminTags;

        private readonly IHttpContextAccessor httpContextAccessor;

        static MarkupTagsFactory()
        {
            adminTags = Array.Empty<MarkupTag>();
            modTags = Array.Empty<MarkupTag>();
            janitorTags = Array.Empty<MarkupTag>();

            publicTags = new IMarkupTag[]
            {
                new StrongTag(),
                new EmphasizeTag(),
                new StrikeTag(),
                new SpoilerTag(),
                new QuoteTag()
            };
        }

        public MarkupTagsFactory(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<IMarkupTag> GetTags()
        {
            var role = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            return GetTagsForUser(role);
        }

        public IEnumerable<IMarkupTag> GetTagsForUser(string role) =>
            role switch
            {
                Administrator => publicTags.Union(janitorTags).Union(modTags).Union(adminTags),
                Moderator => publicTags.Union(janitorTags).Union(modTags),
                Janitor => publicTags.Union(janitorTags),
                _ => publicTags
            };
    }
}
