using OakChan.Services.DTO;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public interface IUserService
    {
        public Task<UserDto> CreateAnonymousAsync(string ip);
    }
}
