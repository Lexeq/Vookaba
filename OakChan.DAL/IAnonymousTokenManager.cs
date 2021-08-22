using OakChan.DAL.Entities;
using System.Threading.Tasks;

namespace OakChan.DAL
{
    public interface IAnonymousTokenManager<TKey>
    {
        public Task<AnonymousToken> CreateGuestTokenAsync();

        public Task<AnonymousToken> CreateUserTokenAsync(TKey userId);

        public Task<AnonymousToken> GetUserToken(TKey userId);

    }
}
