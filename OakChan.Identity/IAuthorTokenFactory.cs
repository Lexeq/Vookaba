using System.Threading.Tasks;

namespace OakChan.Identity
{
    public interface IAuthorTokenFactory<TAuthorToken>
        where TAuthorToken : AuthorToken
    {
        public Task<TAuthorToken> GenerateTokenAsync();

    }
}
