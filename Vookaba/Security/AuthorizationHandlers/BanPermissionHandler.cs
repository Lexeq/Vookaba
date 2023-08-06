#nullable enable
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Vookaba.Common;
using Vookaba.Utils;
using Vookaba.ViewModels.Ban;

namespace Vookaba.Security.AuthorizationHandlers
{
    public class BanPermissionRequirement : IAuthorizationRequirement { }

    public class BanPermissionHandler : AuthorizationHandler<BanPermissionRequirement, BanParams>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BanPermissionRequirement requirement, BanParams resource)
        {
            var role = context.User.FindFirstValue(ClaimTypes.Role);

            if (role is not ApplicationConstants.Roles.Moderator and not ApplicationConstants.Roles.Administrator)
            {
                context.Fail(new AuthorizationFailureReason(this, "Your role have no right to ban users."));
            }
            if (role != ApplicationConstants.Roles.Administrator && resource.IsSubnetBan)
            {
                context.Fail(new AuthorizationFailureReason(this, "You can not ban subnet."));
            }
            if (!context.User.HasBoardPermission(resource.Board))
            {
                context.Fail(new AuthorizationFailureReason(this, "The user does not have permisson to moderate this board."));
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
