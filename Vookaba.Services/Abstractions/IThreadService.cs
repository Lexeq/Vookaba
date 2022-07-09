namespace Vookaba.Services.Abstractions
{
    public interface IThreadService
    {
        /// <summary>
        /// Создает новый тред на доске.
        /// </summary>
        /// <param name="boardKey">ID доски, на которой будет создан тред.</param>
        /// <param name="threadCreationDto">Данные для создания треда.</param>
        /// <returns>Информация о созданном треде.</returns>
        public Task<ThreadDto> CreateThreadAsync(string boardKey, ThreadCreationDto threadCreationDto);


        /// <summary>
        /// Возвращает тред.
        /// </summary>
        ///<param name="boardKey">ID доски.</param>
        /// <param name="threadId">ID треда.</param>
        /// <returns>Тред или null, если тред не существует.</returns>
        public Task<ThreadDto?> GetThreadAsync(string boardKey, int threadId);

        /// <summary>
        /// Создает новый пост в треде.
        /// </summary>
        /// <param name="threadId">ID треда.</param>
        /// <param name="post">Данные поста.</param>
        /// <returns>Данные созанного поста.</returns>
        public Task<PostDto> AddPostToThreadAsync(int threadId, PostCreationDto post);

        /// <summary>
        /// Возвращает информацию о треде.
        /// </summary>
        /// <param name="boardKey">ID доски.</param>
        /// <param name="threadId">ID треда.</param>
        /// <returns>Объект с ифнормацией о треде или null, если тред не существует.</returns>
        public Task<ThreadInfoDto?> GetThreadInfoAsync(string boardKey, int threadId);

        /// <summary>
        /// Управляет закреплением треда.
        /// </summary>
        /// <param name="threadId">ID треда.</param>
        /// <param name="isPinned">Флаг определяющий, является ли тред закрепленным.</param>
        public Task SetIsPinned(int threadId, bool isPinned);

        /// <summary>
        /// Управляет блокированием треда.
        /// </summary>
        /// <param name="threadId">ID тредв.</param>
        /// <param name="isReadOnly">Флаг определяющий, является ли тред заблокированным.</param>
        /// <returns></returns>
        public Task SetIsReadOnly(int threadId, bool isReadOnly);
    }
}
