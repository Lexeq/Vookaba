using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace OakChan.Deanon
{
    public class DeanonClaimHandler : AuthorizationHandler<DeanonRequirement>
    {
        private readonly DeanonOptions options;

        public DeanonClaimHandler(IOptions<DeanonOptions> options)
        {
            this.options = options.Value;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DeanonRequirement requirement)
        {
            if (context.User.HasClaim(claim => claim.Type == options.IdTokenClaimType))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
