using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OakChan.Utils;
using System.Threading.Tasks;

namespace OakChan.Security.AuthorizationHandlers
{
    public class BoardPermissionRequirement : IAuthorizationRequirement { }

    public class BoardPermissionHandler : AuthorizationHandler<BoardPermissionRequirement>
    {
        private readonly IHttpContextAccessor accessor;
        private readonly ILogger<BoardPermissionHandler> logger;

        public BoardPermissionHandler(IHttpContextAccessor accessor, ILogger<BoardPermissionHandler> logger)
        {
            this.accessor = accessor;
            this.logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BoardPermissionRequirement requirement)
        {
            var board = accessor.HttpContext.Request.RouteValues["board"]?.ToString();
            if(board == null)
            {
                board = accessor.HttpContext.Request.Query["board"];
            }
            if (board != null)
            {
                if (context.User.HasBoardPermission(board))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }
            else
            {
                logger.LogInformation("Route value for board was not provided.");
            }
            context.Fail();
            return Task.CompletedTask;
        }
    }
}
