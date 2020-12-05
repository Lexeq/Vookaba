using OakChan.DAL.Entities;
using OakChan.Services.DTO;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public interface IThreadService
    {
        public Task<Thread> GetThreadAsync(string board, int thread);

        public Task<Post> CreatePostAsync(string board, int thread, PostCreationData post);
    }
}
