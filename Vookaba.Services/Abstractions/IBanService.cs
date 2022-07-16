using System.Net;

namespace Vookaba.Services.Abstractions
{
    public interface IBanService
    {
        /// <summary>
        /// Создает новый бан.
        /// </summary>
        /// <param name="ban">Объект, содержащий параметры бана.</param>
        public Task CreateAsync(BanCreationDto ban);

        /// <summary>
        /// Снимает существующий бан.
        /// </summary>
        /// <param name="banId">Id бана, который нужно отменить.</param>
        public Task RemoveAsync(int banId);

        /// <summary>
        /// Возвращает объект бана.
        /// </summary>
        /// <param name="banId">Id бана.</param>
        /// <returns>Объект бана или <c>null</c>, если бан не найден.</returns>
        public Task<BanDto?> GetAsync(int banId);

        /// <summary>
        /// Проверяет существует ли активный бан.
        /// </summary>
        /// <param name="address">IP адресс, для которого осужествлется проверка.</param>
        /// <param name="authorToken">AuthorToken пользователя, для которого осуществляется проверка.</param>
        /// <param name="boardKey">Доска, для которой осуществяется проверка.</param>
        /// <returns>Объект бана или <c>null</c>, если активного бана не существует.</returns>
        public Task<BanInfoDto?> FindActiveBan(IPAddress address, Guid authorToken, string boardKey);
    }

}
