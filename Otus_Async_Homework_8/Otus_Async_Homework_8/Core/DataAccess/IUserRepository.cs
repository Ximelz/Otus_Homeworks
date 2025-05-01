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
        /// <returns>Возвращает пользователя если он найден, null если нет.</returns>
        ToDoUser? GetUser(Guid userId);

        /// <summary>
        /// Метод получения пользователя по telegram id
        /// </summary>
        /// <param name="telegramUserId"></param>
        /// <returns>Возвращает пользователя если он найден, null если нет.</returns>
        ToDoUser? GetUserByTelegramUserId(long telegramUserId);

        /// <summary>
        /// Метод добавления пользователя.
        /// </summary>
        /// <param name="user"></param>
        void Add(ToDoUser user);
    }
}
