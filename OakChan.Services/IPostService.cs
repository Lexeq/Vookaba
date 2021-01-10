using OakChan.Services.DTO;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public interface IPostService
    {
        Task<PostDto> CreatePost(ThreadDto threadDto, PostCreationDto postDto);
    }
}