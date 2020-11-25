using OakChan.DAL.Entities;
using System.Threading.Tasks;

namespace OakChan.Models.Interfaces
{
    public interface IUserService
    {
        public Task<Anonymous> CreateAnonymousAsync(string ip);
    }
}
