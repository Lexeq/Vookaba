using OakChan.DAL.Entities;
using OakChan.Services.DTO;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public interface IThreadService
    {
        public Task<ThreadDto> GetThreadAsync(string boardId, int threadId);

        public Task<Post> CreatePostAsync(string board, int thread, PostCreationData post);
    }
}
