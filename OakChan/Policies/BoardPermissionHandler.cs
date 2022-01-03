using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OakChan.Common;
using System.Threading.Tasks;

namespace OakChan.Policies
{
    public class BoardPermissionRequirement : IAuthorizationRequirement { }

    public class BoardPermissionHandler : AuthorizationHandler<BoardPermissionRequirement>
    {
        private readonly IHttpContextAccessor accessor;

        public BoardPermissionHandler(IHttpContextAccessor accessor)
        {
            this.accessor = accessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BoardPermissionRequirement requirement)
        {
            var board = accessor.HttpContext.Request.RouteValues["board"]?.ToString();
            if (board != null &&
                (context.User.IsInRole(OakConstants.Roles.Administrator) ||
                context.User.HasClaim(OakConstants.ClaimTypes.BoardPermission, board)))

            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}
