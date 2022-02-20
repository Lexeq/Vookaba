using OakChan.Services.DTO;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public interface IPostProcessor
    {
        public Task ProcessAsync(PostCreationDto post);
    }
}
