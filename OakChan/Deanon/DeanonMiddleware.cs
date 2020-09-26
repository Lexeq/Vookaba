using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using OakChan.Models.Interfces;
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
        private readonly IUserService users;

        private string _scheme;
        private string _claim;

        public DeanonMiddleware(RequestDelegate next, IUserService users)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.users = users ?? throw new ArgumentNullException(nameof(users));
            _scheme = DeanonDefaults.AuthenticationScheme;
            _claim = DeanonDefaults.UidClaimName;
        }

        public async Task InvokeAsync(HttpContext context)
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
