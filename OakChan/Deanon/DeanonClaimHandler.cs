using Microsoft.AspNetCore.Authorization;
using OakChan.Common;
using System.Threading.Tasks;

namespace OakChan.Deanon
{
    public class DeanonClaimHandler : AuthorizationHandler<DeanonRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DeanonRequirement requirement)
        {
            if (context.User.HasClaim(claim => claim.Type == OakConstants.ClaimTypes.AuthorToken))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
