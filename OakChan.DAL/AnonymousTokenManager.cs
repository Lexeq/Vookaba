using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OakChan.DAL.Database;
using OakChan.DAL.Entities;
using System;
using System.Net;
using System.Threading.Tasks;

namespace OakChan.DAL
{
    public class AnonymousTokenManager : IAnonymousTokenManager<int>
    {
        private readonly OakDbContext context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AnonymousTokenManager(OakDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task<AnonymousToken> CreateGuestTokenAsync()
            => CreateToken(null);

        public Task<AnonymousToken> CreateUserTokenAsync(int userId)
            => CreateToken(userId);

        public Task<AnonymousToken> GetUserToken(int userId)
        {
            return context.AnonymousTokens.SingleAsync(t => t.UserId == userId);
        }

        private async Task<AnonymousToken> CreateToken(int? userId)
        {
            var token = new AnonymousToken
            {
                Created = DateTime.UtcNow,
                Token = Guid.NewGuid(),
                UserId = userId,
                IP = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress ?? IPAddress.Loopback
            };
            context.AnonymousTokens.Add(token);
            await context.SaveChangesAsync();
            return token;
        }
    }

}
