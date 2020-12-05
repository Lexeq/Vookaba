using OakChan.DAL.Entities;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public interface IUserService
    {
        public Task<Anonymous> CreateAnonymousAsync(string ip);
    }
}
