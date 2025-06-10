using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Concurrent_Homework_12
{
    public interface IUserService
    {
        /// <summary>
        /// Регистрация нового пользователя.
        /// </summary>
        /// <param name="telegramUserId">id пользователя из telegram</param>
        /// <param name="telegramUserName">Имя пользователя из telegram</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Объект нового пользователя.</returns>
        Task<ToDoUser> RegisterUser(long telegramUserId, string telegramUserName, CancellationToken ct);

        /// <summary>
        /// Получения уже авторизированного пользователя.
        /// </summary>
        /// <param name="telegramUserId">id пользователя из telegram</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Объект пользователя.</returns>
        Task<ToDoUser?> GetUser(long telegramUserId, CancellationToken ct);
    }
}
