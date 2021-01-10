using OakChan.DAL.Entities;
using OakChan.Services.DTO;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public interface IThreadService
    {
        public Task<ThreadDto> GetThreadAsync(string boardId, int threadId);

        public Task<PostDto> AddPostToThreadAsync(string boardId, int threadId, PostCreationDto post);
    }
}
