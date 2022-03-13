using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

namespace OakChan.Deanon
{
    public static class DeanonExtensions
    {
        public static IApplicationBuilder UseDeanon(this IApplicationBuilder app)
            => UseDeanon(app, new DeanonOptions());

        public static IApplicationBuilder UseDeanon(this IApplicationBuilder app, DeanonOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            return app.UseMiddleware<DeanonMiddleware>(Options.Create(options));
        }

        public static IServiceCollection AddDeanon(this IServiceCollection services)
        {
            services.AddAuthentication()
                .AddCookie(DeanonConstants.AuthenticationScheme, o =>
                {
                    o.Cookie.Name = "greeting";
                    o.Cookie.IsEssential = true;
                    o.LoginPath = "/";
                    o.AccessDeniedPath = "/";
                    o.Cookie.SameSite = SameSiteMode.Strict;
                    o.SlidingExpiration = true;
                });

            return services;
        }
    }
}
