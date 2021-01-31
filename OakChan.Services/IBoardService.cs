using OakChan.Services.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public interface IBoardService
    {
        public Task<ThreadDto> CreateThreadAsync(string boardId, ThreadCreationDto th);

        public Task<IEnumerable<BoardInfoDto>> GetBoardsAsync(bool showHidden);

        public Task<BoardInfoDto> GetBoardInfoAsync(string boardId);

        public Task<BoardPageDto> GetBoardPageAsync(string boardId, int page, int pageSize);
    }
}
