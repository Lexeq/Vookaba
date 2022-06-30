using Vookaba.Services.DTO;
using System.Threading.Tasks;

namespace Vookaba.Services
{
    public interface IPostProcessor
    {
        public Task ProcessAsync(PostCreationDto post);
    }
}
