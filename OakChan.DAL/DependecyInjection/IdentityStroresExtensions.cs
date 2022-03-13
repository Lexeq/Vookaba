using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OakChan.DAL.Database;
using OakChan.DAL.Identity;
using OakChan.Identity;

namespace OakChan.Extensions.DependencyInjection
{
    public static class IdentityStroresExtensions
    {
        public static IdentityBuilder AddDbStores(this IdentityBuilder builder)
        {
            builder.AddUserStore<ApplicationUserStore<OakDbContext>>();
            builder.AddRoleStore<ApplicationRoleStore<OakDbContext>>();
            builder.AddUserManager<ApplicationUserManager>();

            var services = builder.Services;

            services.AddScoped(s => (ApplicationUserStore<OakDbContext>)s.GetRequiredService<IUserStore<ApplicationUser>>());
            services.AddScoped<IInvitationStore<ApplicationInvitation>>(s => s.GetRequiredService<ApplicationUserStore<OakDbContext>>());
            services.AddScoped<IUserInvitationStore<ApplicationUser>>(s => s.GetRequiredService<ApplicationUserStore<OakDbContext>>());
            services.AddScoped<IAuthorTokenStore<AuthorToken>, AuthorTokenStore<AuthorToken, OakDbContext>>();

            return builder;
        }
    }
}
