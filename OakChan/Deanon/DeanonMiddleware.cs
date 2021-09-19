using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OakChan.Common;
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

        public async Task InvokeAsync(HttpContext context, IAuthorTokenManager postingTokens)
        {
            if (!context.User.HasClaim(c => c.Type == OakConstants.ClaimTypes.AuthorToken))
            {
                var anonAuth = await context.AuthenticateAsync(DeanonConstants.AuthenticationScheme);

                if (!anonAuth.Succeeded)
                {
                    var token = await postingTokens.CreateTokenAsync();
                    var identity = new ClaimsIdentity(
                        claims: new[] { new Claim(OakConstants.ClaimTypes.AuthorToken, token.Token.ToString()) },
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
            await next.Invoke(context);
        }
    }
}
