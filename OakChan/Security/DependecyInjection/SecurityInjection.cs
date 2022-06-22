using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using OakChan.Common;
using OakChan.Security.AuthorizationHandlers;
using PoliciesNames = OakChan.Common.OakConstants.Policies;

namespace OakChan.Security.DependecyInjection
{
    public static class SecurityInjection
    {
        public static IServiceCollection AddChanPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    name: PoliciesNames.CanInviteUsers,
                    policy =>
                    {
                        policy.RequireRole(OakConstants.Roles.Administrator);
                    });

                options.AddPolicy(PoliciesNames.HasStaffRole, policy =>
                {
                    policy.RequireRole(
                        OakConstants.Roles.Administrator,
                        OakConstants.Roles.Moderator,
                        OakConstants.Roles.Janitor);
                });

                options.AddPolicy(PoliciesNames.HasBoardPermission, policy =>
                {
                    policy.AddRequirements(new BoardPermissionRequirement());
                });

                options.AddPolicy(PoliciesNames.CanEditBoards, policy =>
                {
                    policy.RequireRole(OakConstants.Roles.Administrator);
                });

                options.AddPolicy(PoliciesNames.CanDeletePosts, policy =>
                {
                    policy.AddRequirements(new PostDeletingPermissionRequirement());
                });

                options.AddPolicy(PoliciesNames.CanEditUsers, policy =>
                {
                    policy.RequireRole(OakConstants.Roles.Administrator);
                });

                options.AddPolicy(PoliciesNames.CanPost, policy =>
                {
                    policy.AddRequirements(new PostingPermissionRequirement());
                });

                options.AddPolicy(PoliciesNames.CanEditThreads, policy =>
                {
                    policy.RequireRole(OakConstants.Roles.Administrator, OakConstants.Roles.Moderator);
                    policy.Combine(options.GetPolicy(PoliciesNames.HasBoardPermission));
                });
            });

            services.AddScoped<IAuthorizationHandler, BoardPermissionHandler>();
            services.AddScoped<IAuthorizationHandler, PostDeletingPermissionHandler>();
            services.AddSingleton<IAuthorizationHandler, PostingPermissionHandler>();
            services.AddScoped<AppCookieEvents>();

            return services;
        }
    }
}
