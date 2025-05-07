using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Async_Homework_8
{
    public interface IUserRepository
    {
        /// <summary>
        /// Метод получения пользователя по guid id.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Возвращает пользователя если он найден, null если нет.</returns>
        Task<ToDoUser?> GetUser(Guid userId, CancellationToken ct);

        /// <summary>
        /// Метод получения пользователя по telegram id
        /// </summary>
        /// <param name="telegramUserId"></param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Возвращает пользователя если он найден, null если нет.</returns>
        Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct);

        /// <summary>
        /// Метод добавления пользователя.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="ct">Объект отмены задачи.</param>
        Task Add(ToDoUser user, CancellationToken ct);
    }
}
