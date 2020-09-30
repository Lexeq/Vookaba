using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using OakChan.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OakChan.Deanon
{
    public class DeanonMiddleware
    {
        private readonly RequestDelegate next;

        private string _scheme;
        private string _claim;

        public DeanonMiddleware(RequestDelegate next)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            _scheme = DeanonDefaults.AuthenticationScheme;
            _claim = DeanonDefaults.UidClaimName;
        }

        public async Task InvokeAsync(HttpContext context, IUserService users)
        {
            var authResult = await context.AuthenticateAsync(_scheme);
            if (!authResult.Succeeded)
            {
                var user = await users.CreateAnonymousAsync(context.Connection.RemoteIpAddress.ToString());

                var identity = new ClaimsIdentity(
                    claims: new[] { new Claim(_claim, user.Id.ToString()) },
                    authenticationType: _scheme);

                var principal = new ClaimsPrincipal(new[] { identity });
                await context.SignInAsync(_scheme, principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = new DateTime(2099, 1, 1),
                    });
            }
            await next.Invoke(context);
        }
    }
}
