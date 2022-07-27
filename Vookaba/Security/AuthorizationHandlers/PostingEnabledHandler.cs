using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Vookaba.Common;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Vookaba.Security.AuthorizationHandlers
{
    public class PostingEnabledRequirement : IAuthorizationRequirement { }

    public class PostingEnabledHandler : AuthorizationHandler<PostingEnabledRequirement>
    {
        private readonly ApplicationOptions appOptions;

        public PostingEnabledHandler(IOptions<ApplicationOptions> appOptions)
        {
            this.appOptions = appOptions.Value;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PostingEnabledRequirement requirement)
        {
            if (!appOptions.IsAnonymousPostingAllowed && !context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                context.Fail(new AuthorizationFailureReason(this, "Only registered users can post."));
            }
            else
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
