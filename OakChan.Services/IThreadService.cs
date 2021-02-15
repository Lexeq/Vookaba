using OakChan.Services.DTO;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public interface IThreadService
    {
        public Task<ThreadBoardAggregationDto> GetThreadAsync(string boardId, int threadId);

        public Task<PostDto> AddPostToThreadAsync(string boardId, int threadId, PostCreationDto post);
    }
}
