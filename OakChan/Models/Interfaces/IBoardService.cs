using OakChan.Models.DB;
using OakChan.Models.DB.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OakChan.Models.Interfaces
{
    public interface IBoardService
    {
        public Task<BoardPreview> GetBoardPreviewAsync(string boardId, int page, int threadsPerPage);

        public Task<Thread> CreateThreadAsync(string boardId, PostCreationData data);

        public Task<IEnumerable<Board>> GetBoardsAsync();
    }
}
