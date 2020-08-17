using OakChan.Models.DB;
using OakChan.Models.DB.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OakChan.Models.Interfces
{
    public interface IBoardService
    {
        public Task<Board> GetBoardPreviewAsync(string boardId, int page, int threadsPerPage);

        public Task<Thread> CreateThreadAsync(string boardId, PostCreationData data);

        public Task<IEnumerable<Board>> GetBoardsAsync();
    }
}
