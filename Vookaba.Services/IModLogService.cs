#nullable enable
using Vookaba.Common;
using Vookaba.Services.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vookaba.Services
{
    public interface IModLogService
    {
        /// <summary>
        /// Записывает действия с имиджбордой для текущего пользователя.
        /// </summary>
        /// <param name="eventId">Тип действия.</param>
        /// <param name="entityId">Id объекта для которого производится действие.</param>
        /// <param name="note">Дополнительное примечание к действию.</param>
        public Task LogAsync(ApplicationEvent eventId, string entityId, string? note = null);

        /// <summary>
        /// Возвращает дейтсвия пользователя с имиджбордой.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <param name="offset">Смещение, необходимое для выборки определенного подмножества логов.</param>
        /// <param name="count">Количество возвращаемых логов.</param>
        /// <param name="lastFirst">Порядок сортировки логов.
        /// Если <c>true</c>, то последние днйствия будут в начале списка. Иначе перечисление в хронологическом порядке.
        /// По умолчанию <c>true</c>.</param>
        /// <returns>Перечисление объектов с информацией о действии.</returns>
        public Task<IEnumerable<ModLogDto>> GetLogsForUserAsync(int userId, int offset, int count, bool lastFirst = true);
    }
}