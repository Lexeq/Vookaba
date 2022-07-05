using Vookaba.Services.DTO;
using System.Threading.Tasks;

namespace Vookaba.Services.Abstractions
{
    public interface IPostProcessor
    {
        public Task ProcessAsync(PostCreationDto post);
    }
}
