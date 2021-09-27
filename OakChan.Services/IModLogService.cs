#nullable enable
using OakChan.Common;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public interface IModLogService
    {
        /// <summary>
        /// Записывает действия с имиджбордой.
        /// </summary>
        /// <param name="eventId">Тип действия.</param>
        /// <param name="entityId">Id объекта для которого производится действие.</param>
        public Task LogAsync(ApplicationEvent eventId, string entityId);


        /// <summary>
        /// Записывает действия с имиджбордой.
        /// </summary>
        /// <param name="eventId">Тип действия.</param>
        /// <param name="entityId">Id объекта для которого производится действие.</param>
        /// <param name="note">Дополнительное примечание к действию.</param>
        public Task LogAsync(ApplicationEvent eventId, string entityId, string? note);
    }
}