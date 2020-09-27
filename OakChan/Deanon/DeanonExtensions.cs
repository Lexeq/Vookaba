using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

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

        public static async Task<Guid> GetAnonGuidAsync(this HttpContext context)
        {
            var authResult = await context.AuthenticateAsync(DeanonDefaults.AuthenticationScheme);
            if (authResult.Succeeded)
            {
                var claim = authResult.Principal.FindFirst(DeanonDefaults.UidClaimName);
                if (claim != null && Guid.TryParse(claim.Value, out var guid))
                {
                    return guid;
                }
            }
            throw new DeanonException("Can't get Guid");
        }
    }
}
