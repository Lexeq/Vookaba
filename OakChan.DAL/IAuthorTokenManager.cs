using OakChan.DAL.Entities;
using OakChan.Identity;
using System.Threading.Tasks;

namespace OakChan.DAL
{
    public interface IAuthorTokenManager
    {
        public Task<AuthorToken> CreateTokenAsync();

        public Task<AuthorToken> GetTokenAsync(ApplicationUser user);

    }
}
