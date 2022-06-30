using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Vookaba.Common;
using Vookaba.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Vookaba.Deanon
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

        public async Task InvokeAsync(HttpContext context, ApplicationUserManager userManager)
        {
            if (!context.User.HasClaim(c => c.Type == ApplicationConstants.ClaimTypes.AuthorToken))
            {
                var anonAuth = await context.AuthenticateAsync(DeanonConstants.AuthenticationScheme);

                if (!anonAuth.Succeeded)
                {
                    var token = await userManager.CreateAnonymousTokenAsync();
                    var identity = new ClaimsIdentity(
                        claims: new[] { new Claim(ApplicationConstants.ClaimTypes.AuthorToken, token.ToString()) },
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
