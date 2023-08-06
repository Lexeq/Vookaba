using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
                options.DefaultPolicy = new AuthorizationPolicyBuilder(IdentityConstants.ApplicationScheme)
                    .RequireAuthenticatedUser()
                    .Build();

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
                    policy.AddRequirements(new PostingEnabledRequirement());
                });

                options.AddPolicy(PoliciesNames.CanEditThreads, policy =>
                {
                    policy.RequireRole(ApplicationConstants.Roles.Administrator, ApplicationConstants.Roles.Moderator);
                    policy.Combine(options.GetPolicy(PoliciesNames.HasBoardPermission));
                });

                options.AddPolicy(PoliciesNames.CanBanUsers, policy =>
                {
                    policy.AddRequirements(new BanPermissionRequirement());
                });
            });

            services.AddScoped<IAuthorizationHandler, BoardPermissionHandler>();
            services.AddScoped<IAuthorizationHandler, PostDeletingPermissionHandler>();
            services.AddSingleton<IAuthorizationHandler, PostingEnabledHandler>();
            services.AddSingleton<IAuthorizationHandler, BanPermissionHandler>();
            services.AddScoped<AppCookieEvents>();

            return services;
        }
    }
}
