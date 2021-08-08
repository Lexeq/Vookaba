using OakChan.Services.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public interface ITopThreadsService
    {
        /// <summary>
        /// Возвращает для каждой доски тред с последним ответом.
        /// </summary>
        /// <remarks>Учитываютяся только посты до бамплимита.</remarks>
        /// <param name="limit">Количество досок.</param>
        public Task<IEnumerable<ThreadPreviewDto>> GetTopThreadsByLastPostAsync(int limit);

        /// <summary>
        /// Возвращает для каждой доски последний созданный тред.
        /// </summary>
        /// <remarks>Учитываютяся только посты до бамплимита.</remarks>
        /// <param name="limit">Количество досок.</param>
        public Task<IEnumerable<ThreadPreviewDto>> GetTopThreadsByCreationTimeAsync(int limit);
    }
}
