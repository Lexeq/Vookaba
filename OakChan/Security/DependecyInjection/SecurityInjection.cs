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
                    policy.Combine(options.GetPolicy(PoliciesNames.HasStaffRole))
                          .Combine(options.GetPolicy(PoliciesNames.HasBoardPermission))
                          .AddRequirements(new PostDeletingPermissionRequirement());

                });

                options.AddPolicy(PoliciesNames.CanEditUsers, policy =>
                {
                    policy.RequireRole(OakConstants.Roles.Administrator);
                });

                options.AddPolicy(PoliciesNames.CanPost, policy =>
                {
                    policy.RequireClaim(OakConstants.ClaimTypes.AuthorToken);
                });
            });

            services.AddScoped<IAuthorizationHandler, BoardPermissionHandler>();
            services.AddScoped<IAuthorizationHandler, PostDeletingPermissionHandler>();
            services.AddScoped<CookieValidator>();
            services.PostConfigure<CookieAuthenticationOptions>(options =>
                options.EventsType = typeof(CookieValidator)
            );

            return services;
        }
    }
}
