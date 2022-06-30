using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Vookaba.Identity
{
    public class AuthorTokenFactory<TAuthorToken> : IAuthorTokenFactory<TAuthorToken>
        where TAuthorToken : AuthorToken, new()
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthorTokenFactory(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public virtual Task<TAuthorToken> GenerateTokenAsync()
        {
            var token = new TAuthorToken
            {
                Token = Guid.NewGuid(),
                IP = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress ?? IPAddress.Loopback,
                Created = DateTime.UtcNow
            };
            return Task.FromResult(token);
        }
    }
}
