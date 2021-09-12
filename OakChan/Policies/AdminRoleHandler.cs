using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace OakChan.Policies
{
    public class AdminRoleHandler : AuthorizationHandler<RoleRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RoleRequirement requirement)
        {
            if (context.User.IsInRole(requirement.RoleName))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
