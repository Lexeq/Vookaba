using OakChan.DAL.Entities;
using OakChan.Services.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public interface IBoardService
    {
        public Task<Thread> CreateThreadAsync(string boardId, PostCreationData data);

        public Task<IEnumerable<Board>> GetBoardsAsync();

        public Task<BoardInfoDto> GetBoardAsync(string board);

        public Task<BoardPageDto> GetBoardPageAsync(string board, int page, int threadsPerPage);
    }
}
