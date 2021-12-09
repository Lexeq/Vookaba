using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using OakChan.Common;

namespace OakChan.Policies
{
    public static class PolicyDI
    {
        public static IServiceCollection AddChanPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    name: OakConstants.Policies.CanInviteUsers,
                    policy =>
                    {
                        policy.RequireRole(OakConstants.Roles.Administrator);
                    });

                options.AddPolicy(OakConstants.Policies.HasBoardPermission, policy =>
                {
                    policy.AddRequirements(new BoardPermissionRequirement());
                });

                options.AddPolicy(OakConstants.Policies.CanEditBoards, policy =>
                {
                    policy.RequireRole(OakConstants.Roles.Administrator);
                });

                options.AddPolicy(OakConstants.Policies.CanDeletePosts, policy =>
                {
                    policy.RequireRole(
                        OakConstants.Roles.Administrator,
                        OakConstants.Roles.Moderator,
                        OakConstants.Roles.Janitor);

                    policy.Combine(options.GetPolicy(OakConstants.Policies.HasBoardPermission));

                    policy.AddRequirements(new PostDeletingPermissionRequirement());

                });

                options.AddPolicy(OakConstants.Policies.CanEditUsers, policy =>
                {
                    policy.RequireRole(OakConstants.Roles.Administrator);
                });

                options.AddPolicy(OakConstants.Policies.CanPost, policy =>
                {
                    policy.RequireClaim(OakConstants.ClaimTypes.AuthorToken);
                });
            });

            services.AddSingleton<IAuthorizationHandler, BoardPermissionHandler>();
            services.AddSingleton<IAuthorizationHandler, PostDeletingPermissionHandler>();
            return services;
        }
    }
}
