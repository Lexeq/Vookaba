using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using OakChan.Models.Interfces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OakChan
{
    public class DeanonMiddleware
    {
        public const string AuthenticationScheme = "Deanon";
        public const string UidClaimName = "uid";

        private readonly RequestDelegate next;
        private readonly IUserService users;

        public DeanonMiddleware(RequestDelegate next, IUserService users)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.users = users ?? throw new ArgumentNullException(nameof(users)); ;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authResult = await context.AuthenticateAsync(AuthenticationScheme);
            if (!authResult.Succeeded)
            {
                var user = users.CreateAnonymous();

                var identity = new ClaimsIdentity(
                    claims: new[] { new Claim(UidClaimName, user.Id.ToString()) },
                    authenticationType: AuthenticationScheme);

                var principal = new ClaimsPrincipal(new[] { identity });
                await context.SignInAsync(AuthenticationScheme, principal);
            }
            await next.Invoke(context);
        }
    }
}
