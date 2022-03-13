using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OakChan.Identity;

namespace OakChan.Extensions.DependencyInjection
{
    public static class ChanIdentityExtensions
    {
        public static IdentityBuilder AddChanIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>();

            services.AddScoped<InvitationManager<ApplicationInvitation>>();
            services.AddScoped<IAuthorTokenFactory<AuthorToken>, AuthorTokenFactory<AuthorToken>>();

            return builder;
        }
    }
}
