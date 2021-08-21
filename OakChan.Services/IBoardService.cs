#nullable enable
using OakChan.Services.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public interface IBoardService
    {
        /// <summary>
        /// Возвращает перечисление, содержащие информацию о существующих досках.
        /// </summary>
        /// <param name="showAll">Если <c>true</c> возвращает также скрытые и неактивные доски.</param>
        public Task<IEnumerable<BoardInfoDto>> GetBoardsAsync(bool showAll);

        /// <summary>
        /// Возвращает информацию о доске по строковому ключу.
        /// </summary>
        /// <param name="boardKey">Строковый идентификатор доски.</param>
        /// <returns>Объект, сожержащий информацию о доске или null, если доска не существует.</returns>
        public Task<BoardInfoDto?> GetBoardInfoAsync(string boardKey);


        /// <summary>
        /// Возвращает объекты предпросмотра тредов.
        /// </summary>
        /// <param name="boardId">ID доски.</param>
        /// <param name="offset">Смещение, необходимое для выборки определенного подмножества тредов.</param>
        /// <param name="count">Количество тредов.</param>
        /// <param name="recentPostsCount">Количество последних ответов для предпросмотра.</param>
        /// <returns>Коллекция объектов предпросмотра тредов или пустое множество, если тредов для задданных параметров не существует.</returns>
        public Task<IEnumerable<ThreadPreviewDto>> GetThreadPreviewsAsync(string boardKey, int offset, int count, int recentPostsCount);

        /// <summary>
        /// Создает новую доску.
        /// </summary>
        /// <param name="board">Параметры новой доски</param>
        public Task CreateBoardAsync(BoardDto board);

        /// <summary>
        /// Удаляет существующкю доску.
        /// </summary>
        /// <param name="boardKey">Идентификатор доски.</param>
        public Task DeleteBoardAsync(string boardKey);

        /// <summary>
        /// Обновляет параметры треда.
        /// </summary>
        /// <param name="key">Идентификатор доски.</param>
        /// <param name="updatedBoard">Новые параметры доски.</param>
        public Task UpdateBoardAsync(string key, BoardDto updatedBoard);
    }
}