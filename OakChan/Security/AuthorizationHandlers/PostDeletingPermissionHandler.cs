using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OakChan.Common;
using OakChan.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace OakChan.Security.AuthorizationHandlers
{
    public class PostDeletingPermissionRequirement : IAuthorizationRequirement { }

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

            var area = (await ReadViewModelAsync(context.Resource as HttpContext)).Area;
            var role = context.User.FindFirstValue(ClaimTypes.Role);

            if (!Enum.IsDefined(area))
            {
                context.Fail();
                logger.LogWarning($"Bad enum '{area.GetType().Name}: {area}'.");
                return;
            }
            if (role == OakConstants.Roles.Administrator)
            {
                context.Succeed(requirement);
            }
            else if (role == OakConstants.Roles.Moderator
                     && area != PostsDeletionViewModel.DeletingArea.All)
            {
                context.Succeed(requirement);
            }
            else if (role == OakConstants.Roles.Janitor
                     && area == PostsDeletionViewModel.DeletingArea.Single)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }

        private async Task<PostsDeletionViewModel> ReadViewModelAsync(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Request.EnableBuffering();
            var vm = await context.Request.ReadFromJsonAsync<PostsDeletionViewModel>();
            context.Request.Body.Position = 0;
            return vm;
        }
    }
}
