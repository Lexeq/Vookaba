using System.Threading.Tasks;

namespace Vookaba.Identity
{
    public interface IAuthorTokenFactory<TAuthorToken>
        where TAuthorToken : AuthorToken
    {
        public Task<TAuthorToken> GenerateTokenAsync();

    }
}
