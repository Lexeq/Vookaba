using OakChan.DAL.Entities;
using OakChan.Models;
using OakChan.Models.DB.Entities;
using OakChan.Services.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public interface IBoardService
    {
        public Task<BoardPreview> GetBoardPreviewAsync(string boardId, int threadsOffset, int threadsCount);

        public Task<Thread> CreateThreadAsync(string boardId, PostCreationData data);

        public Task<IEnumerable<Board>> GetBoardsAsync();
    }
}
