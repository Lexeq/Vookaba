using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using OakChan.Common;

namespace OakChan.Policies
{
    public static class CanInvitePolicy
    {
        public static IServiceCollection AddChanPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    name: "CanInvite",
                    policy =>
                    {
                        policy.AddRequirements(new RoleRequirement(OakConstants.Roles.Administrator));
                    });
            });

            services.AddSingleton<IAuthorizationHandler, AdminRoleHandler>();
            return services;
        }
    }
}
