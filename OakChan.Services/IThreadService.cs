#nullable enable
using OakChan.Services.DbServices;
using OakChan.Services.DTO;
using System.Threading.Tasks;

namespace OakChan.Services
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
        /// <returns>Объект с ифнормацией о треде</returns>
        public Task<ThreadInfoDto> GetThreadInfoAsync(string boardKey, int threadId);
    }
}
