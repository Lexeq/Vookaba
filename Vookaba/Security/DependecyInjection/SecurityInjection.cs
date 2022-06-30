using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Vookaba.Common;
using Vookaba.Security.AuthorizationHandlers;
using PoliciesNames = Vookaba.Common.ApplicationConstants.Policies;

namespace Vookaba.Security.DependecyInjection
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
                        policy.RequireRole(ApplicationConstants.Roles.Administrator);
                    });

                options.AddPolicy(PoliciesNames.HasStaffRole, policy =>
                {
                    policy.RequireRole(
                        ApplicationConstants.Roles.Administrator,
                        ApplicationConstants.Roles.Moderator,
                        ApplicationConstants.Roles.Janitor);
                });

                options.AddPolicy(PoliciesNames.HasBoardPermission, policy =>
                {
                    policy.AddRequirements(new BoardPermissionRequirement());
                });

                options.AddPolicy(PoliciesNames.CanEditBoards, policy =>
                {
                    policy.RequireRole(ApplicationConstants.Roles.Administrator);
                });

                options.AddPolicy(PoliciesNames.CanDeletePosts, policy =>
                {
                    policy.AddRequirements(new PostDeletingPermissionRequirement());
                });

                options.AddPolicy(PoliciesNames.CanEditUsers, policy =>
                {
                    policy.RequireRole(ApplicationConstants.Roles.Administrator);
                });

                options.AddPolicy(PoliciesNames.CanPost, policy =>
                {
                    policy.AddRequirements(new PostingPermissionRequirement());
                });

                options.AddPolicy(PoliciesNames.CanEditThreads, policy =>
                {
                    policy.RequireRole(ApplicationConstants.Roles.Administrator, ApplicationConstants.Roles.Moderator);
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
