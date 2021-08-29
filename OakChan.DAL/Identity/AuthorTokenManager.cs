using Microsoft.AspNetCore.Http;
using OakChan.DAL;
using OakChan.DAL.Database;
using OakChan.DAL.Entities;
using System;
using System.Net;
using System.Threading.Tasks;

namespace OakChan.Identity
{
    public class AuthorTokenManager : IAuthorTokenFactory, IAuthorTokenManager
    {
        private readonly OakDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthorTokenManager(OakDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthorToken> CreateTokenAsync()
        {
            var token = new AuthorToken
            {
                Token = Guid.NewGuid(),
                IP = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress ?? IPAddress.Loopback
            };
            context.AuthorTokens.Add(token);
            await context.SaveChangesAsync();
            return token;
        }

        public async Task<AuthorToken> GetTokenAsync(ApplicationUser user)
        {
            return await context.AuthorTokens.FindAsync(user.AuthorToken);
        }
    }

}
