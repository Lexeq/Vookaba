﻿using Microsoft.AspNetCore.Authorization;
using OakChan.Common;
using OakChan.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using OakChan.Utils;

namespace OakChan.Security.AuthorizationHandlers
{
    public class PostDeletingPermissionRequirement : IAuthorizationRequirement { }

    public class PostDeletingPermissionHandler : AuthorizationHandler<PostDeletingPermissionRequirement, PostsDeletionOptions>
    {
        private readonly ILogger<PostDeletingPermissionHandler> logger;

        public PostDeletingPermissionHandler(ILogger<PostDeletingPermissionHandler> logger)
        {
            this.logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PostDeletingPermissionRequirement requirement, PostsDeletionOptions options)
        {
            var area = options.Area;
            var role = context.User.FindFirstValue(ClaimTypes.Role);

            if (!Enum.IsDefined(area))
            {
                logger.LogWarning($"Bad enum '{nameof(area)}: {area}'.");
                context.Fail();
                return Task.CompletedTask;
            }

            if (!context.User.HasBoardPermission(options.Board))
            {
                context.Fail();
            }
            if (role == OakConstants.Roles.Administrator)
            {
                context.Succeed(requirement);
            }
            else if (role == OakConstants.Roles.Moderator
                     && area != PostsDeletionOptions.DeletingArea.All)
            {
                context.Succeed(requirement);
            }
            else if (role == OakConstants.Roles.Janitor
                     && area == PostsDeletionOptions.DeletingArea.Single)
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
