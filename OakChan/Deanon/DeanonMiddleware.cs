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

        public async Task InvokeAsync(HttpContext context, AnonymousTokenManager db)
        {
            if (!context.User.HasClaim(c => c.Type == options.IdTokenClaimType))
            {
                var anonAuth = await context.AuthenticateAsync(DeanonConstants.AuthenticationScheme);

                if (!anonAuth.Succeeded)
                {
                    var token = await db.CreateGuestTokenAsync();
                    var identity = new ClaimsIdentity(
                        claims: new[] { new Claim(options.IdTokenClaimType, token.Token.ToString()) },
                        authenticationType: DeanonConstants.AuthenticationScheme);
                    await context.SignInAsync(DeanonConstants.AuthenticationScheme,
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
                await context.SignOutAsync(DeanonConstants.AuthenticationScheme);
            }

            SetDeanonFeature(context);
            await next.Invoke(context);
        }

        private void SetDeanonFeature(HttpContext context)
        {
            var feature = new DeanonFeature
            {
                IPAddress = context.Connection.RemoteIpAddress,
                UserToken = Guid.Parse(context.User.FindFirstValue(options.IdTokenClaimType)),
                UserAgent = context.Request.Headers["User-Agent"]
            };
            context.Features.Set<IDeanonFeature>(feature);
        }
    }
}
