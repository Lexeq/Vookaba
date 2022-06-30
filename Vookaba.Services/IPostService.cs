using Vookaba.Services.DTO;
using System;
using System.Threading.Tasks;

namespace Vookaba.Services
{
    public interface IPostService
    {
        /// <summary>
        /// Get the post by its number on the board.
        /// </summary>
        /// <param name="board">The board the post belongs to.</param>
        /// <param name="number">Post number.</param>
        /// <returns><c>PostDro</c> object.</returns>
        public Task<PostDto> GetByNumberAsync(string board, int number);


        /// <summary>
        /// Delete the post. If the post is OP the whole thread will be deleted.
        /// </summary>
        /// <param name="id">Post id.</param>
        public Task DeleteByIdAsync(int id);

        /// <summary>
        /// Delete posts by creator.
        /// </summary>
        /// <param name="id">Initial post id.</param>
        /// <param name="mode">Creator searching mode.</param>
        /// <param name="area">Area of searching.</param>
        /// <returns></returns>
        public Task DeleteManyAsync(int id, Mode mode, SearchArea area);

    }

    public enum SearchArea
    {
        /// <summary>
        /// Search in same thread.
        /// </summary>
        Thread = 1,
        /// <summary>
        /// Search in same board.
        /// </summary>
        Board,
        /// <summary>
        /// Search everywhere.
        /// </summary>
        All
    }

    [Flags]
    public enum Mode
    {
        /// <summary>
        /// Search posts with same IP.
        /// </summary>
        IP = 1,
        /// <summary>
        /// Search posts with same AuthorToken.
        /// </summary>
        Token = 2
    }
}
