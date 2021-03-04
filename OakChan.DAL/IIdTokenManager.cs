using OakChan.DAL.Entities;
using System.Threading.Tasks;

namespace OakChan.DAL
{
    public interface IIdTokenManager<TKey>
    {
        public Task<IdToken> CreateGuestTokenAsync();

        public Task<IdToken> CreateUserTokenAsync(TKey userId);

        public Task<IdToken> GetUserToken(TKey userId);

    }
}
