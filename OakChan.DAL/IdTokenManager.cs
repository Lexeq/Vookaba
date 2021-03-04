using Microsoft.EntityFrameworkCore;
using OakChan.DAL.Database;
using OakChan.DAL.Entities;
using System;
using System.Threading.Tasks;

namespace OakChan.DAL
{
    public class IdTokenManager : IIdTokenManager<int>
    {
        private readonly OakDbContext context;

        public IdTokenManager(OakDbContext context)
        {
            this.context = context;
        }

        public Task<IdToken> CreateGuestTokenAsync()
            => CreateToken(null);

        public Task<IdToken> CreateUserTokenAsync(int userId)
            => CreateToken(userId);

        public Task<IdToken> GetUserToken(int userId)
        {
            return context.IdTokens.SingleAsync(t => t.UserId == userId);
        }

        private async Task<IdToken> CreateToken(int? userId)
        {
            var token = new IdToken
            {
                Created = DateTimeOffset.UtcNow,
                Id = Guid.NewGuid(),
                UserId = userId
            };
            context.IdTokens.Add(token);
            await context.SaveChangesAsync();
            return token;
        }
    }

}
