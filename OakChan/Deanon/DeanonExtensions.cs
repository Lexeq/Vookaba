using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace OakChan.Deanon
{
    public static class DeanonExtensions
    {
        public static IApplicationBuilder UseDeanon(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            return app.UseMiddleware<DeanonMiddleware>();
        }
        public static AuthenticationBuilder AddDeanonCookie(this AuthenticationBuilder builder)
        {
            builder.AddCookie(DeanonDefaults.AuthenticationScheme, o =>
            {
                o.Cookie.Name = "greeting";
                o.Cookie.IsEssential = true;
            });
            return builder;
        }

        public static void AddDeanonPolicy(this AuthorizationOptions options)
        {
            options.AddPolicy(
                    name: DeanonDefaults.DeanonPolicy,
                    policy =>
                    {
                        policy.RequireClaim(DeanonDefaults.UidClaimName);
                        policy.AuthenticationSchemes.Add(DeanonDefaults.AuthenticationScheme);
                    });
        }
    }
}
