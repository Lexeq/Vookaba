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

                    policy.AddRequirements(new BoardPermissionRequirement());
                });

                options.AddPolicy(OakConstants.Policies.CanEditUsers, policy =>
                {
                    policy.RequireRole(OakConstants.Roles.Administrator);
                });
            });

            services.AddSingleton<IAuthorizationHandler, BoardPermissionHandler>();
            return services;
        }
    }
}
