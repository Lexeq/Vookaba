using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OakChan.DAL;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OakChan.Deanon
{
    public class DeanonMiddleware
    {
        private readonly RequestDelegate next;
        private readonly DeanonOptions options;

        public DeanonMiddleware(RequestDelegate next, IOptions<DeanonOptions> options)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task InvokeAsync(HttpContext context, IdTokenManager db)
        {
            if (!context.User.HasClaim(c => c.Type == DeanonDefaults.UidClaimName))
            {
                var anonAuth = await context.AuthenticateAsync(DeanonDefaults.AuthenticationScheme);

                if (!anonAuth.Succeeded)
                {
                    var token = await db.CreateGuestTokenAsync();
                    var identity = new ClaimsIdentity(
                        claims: new[] { new Claim(DeanonDefaults.UidClaimName, token.Id.ToString()) },
                        authenticationType: DeanonDefaults.AuthenticationScheme);
                    await context.SignInAsync(DeanonDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity),
                        new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = new DateTime(2099, 1, 1)
                        });
                    context.User.AddIdentity(identity);
                }
                else
                {
                    context.User.AddIdentities(anonAuth.Principal.Identities);
                }
            }
            else if (options.SignOutIfUserAuthentificated)
            {
                await context.SignOutAsync(DeanonDefaults.AuthenticationScheme);
            }

            await next.Invoke(context);
        }
    }
}
