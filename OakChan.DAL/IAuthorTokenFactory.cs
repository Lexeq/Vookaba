using OakChan.DAL.Entities;
using System.Threading.Tasks;

namespace OakChan.DAL
{
    public interface IAuthorTokenFactory
    {
        public Task<AuthorToken> CreateTokenAsync();

    }
}
