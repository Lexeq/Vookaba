using OakChan.Models.DB.Entities;
using System.Threading.Tasks;

namespace OakChan.Models.Interfces
{
    public interface IUserService
    {
        public Task<Anonymous> CreateAnonymousAsync(string ip);
    }
}
