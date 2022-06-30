using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Vookaba.DAL.Database;
using Vookaba.DAL.Identity;
using Vookaba.Identity;

namespace Vookaba.Extensions.DependencyInjection
{
    public static class IdentityStroresExtensions
    {
        public static IdentityBuilder AddDbStores(this IdentityBuilder builder)
        {
            builder.AddUserStore<ApplicationUserStore<VookabaDbContext>>();
            builder.AddRoleStore<ApplicationRoleStore<VookabaDbContext>>();
            builder.AddUserManager<ApplicationUserManager>();

            var services = builder.Services;

            services.AddScoped(s => (ApplicationUserStore<VookabaDbContext>)s.GetRequiredService<IUserStore<ApplicationUser>>());
            services.AddScoped<IInvitationStore<ApplicationInvitation>>(s => s.GetRequiredService<ApplicationUserStore<VookabaDbContext>>());
            services.AddScoped<IUserInvitationStore<ApplicationUser>>(s => s.GetRequiredService<ApplicationUserStore<VookabaDbContext>>());
            services.AddScoped<IAuthorTokenStore<AuthorToken>, AuthorTokenStore<AuthorToken, VookabaDbContext>>();

            return builder;
        }
    }
}
