using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Vookaba.Common;
using Vookaba.Services.DTO;
using Vookaba.Utils;

namespace Vookaba.Security.AuthorizationHandlers
{
    public class BanPermissionRequirement : IAuthorizationRequirement { }

    public class BanPermissionHandler : AuthorizationHandler<BanPermissionRequirement, BanCreationDto>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BanPermissionRequirement requirement, BanCreationDto resource)
        {
            var role = context.User.FindFirstValue(ClaimTypes.Role);
            if (role is not ApplicationConstants.Roles.Moderator and not ApplicationConstants.Roles.Administrator)
            {
                context.Fail(new AuthorizationFailureReason(this, "Your role have no right to ban users."));
            }
            if (!context.User.HasBoardPermission(resource.Board))
            {
                context.Fail(new AuthorizationFailureReason(this, "The user does not have permisson to moderate this board."));
            }
            if(role != ApplicationConstants.Roles.Administrator && string.IsNullOrEmpty(resource.Board))
            {
                context.Fail(new AuthorizationFailureReason(this, "You can ban only on specific board."));
            }
            if(role != ApplicationConstants.Roles.Administrator && resource.BannedNetwork?.Subnet != 32)
            {
                context.Fail(new AuthorizationFailureReason(this, "You can not ban subnet."));
            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
