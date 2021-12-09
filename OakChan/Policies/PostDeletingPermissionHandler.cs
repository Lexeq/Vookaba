using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OakChan.Common;
using OakChan.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace OakChan.Policies
{
    public class PostDeletingPermissionHandler : AuthorizationHandler<PostDeletingPermissionRequirement>
    {
        private readonly ILogger<PostDeletingPermissionHandler> logger;

        public PostDeletingPermissionHandler(ILogger<PostDeletingPermissionHandler> logger)
        {
            this.logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PostDeletingPermissionRequirement requirement)
        {
            if (context.HasFailed)
            {
                return;
            }
            var httpContext = (context.Resource as HttpContext);
            httpContext.Request.EnableBuffering();
            var vm = await httpContext.Request.ReadFromJsonAsync<PostsDeletionViewModel>();
            httpContext.Request.Body.Position = 0;


            var role = context.User.FindFirstValue(ClaimTypes.Role);
            if (!Enum.IsDefined(vm.Area))
            {
                context.Fail();
                logger.LogWarning($"Bad enum '{vm.Area.GetType().Name}: {vm.Area}'.");
                return;
            }
            if (role == OakConstants.Roles.Administrator)
            {
                context.Succeed(requirement);
            }
            else if (role == OakConstants.Roles.Moderator
                    && vm.Area != PostsDeletionViewModel.DeletingArea.All)
            {
                context.Succeed(requirement);
            }
            else if (role == OakConstants.Roles.Janitor
                   && vm.Area == PostsDeletionViewModel.DeletingArea.Single)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}
