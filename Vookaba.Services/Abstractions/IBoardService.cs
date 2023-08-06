namespace Vookaba.Services.Abstractions
{
    public interface IBoardService
    {
        /// <summary>
        /// Возвращает перечисление, содержащие информацию о существующих досках.
        /// </summary>
        /// <param name="showAll">Если <c>true</c> возвращает также скрытые и неактивные доски.</param>
        public Task<IEnumerable<BoardDto>> GetBoardsAsync(bool showAll);

        /// <summary>
        /// Возвращает информацию о доске по строковому ключу.
        /// </summary>
        /// <param name="boardKey">Строковый идентификатор доски.</param>
        /// <returns>Объект, сожержащий информацию о доске или null, если доска не существует.</returns>
        public Task<BoardDto?> GetBoardAsync(string boardKey);


        /// <summary>
        /// Возвращает объекты предпросмотра тредов.
        /// </summary>
        /// <param name="boardId">ID доски.</param>
        /// <param name="offset">Смещение, необходимое для выборки определенного подмножества тредов.</param>
        /// <param name="count">Количество тредов.</param>
        public Task<PartialList<ThreadPreviewDto>> GetThreadPreviewsAsync(string boardKey, int offset, int count);

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