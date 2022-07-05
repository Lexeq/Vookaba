using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Vookaba.Common;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Vookaba.Security.AuthorizationHandlers
{
    public class PostingPermissionRequirement : IAuthorizationRequirement { }

    public class PostingPermissionHandler : AuthorizationHandler<PostingPermissionRequirement>
    {
        private readonly ApplicationOptions appOptions;

        public PostingPermissionHandler(IOptions<ApplicationOptions> appOptions)
        {
            this.appOptions = appOptions.Value;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PostingPermissionRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ApplicationConstants.ClaimTypes.AuthorToken))
            {
                context.Fail();
            }
            else if (!appOptions.IsAnonymousPostingAllowed && !context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                context.Fail();
            }
            else
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
