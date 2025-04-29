using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Otus.ToDoList.ConsoleBot.Types;
using Otus_Interfaces_Homework_6;

/*
 * Добавление репозитория IUserRepository
 * Добавить интерфейс IUserRepository
 * interface IUserRepository
 * {
 *     ToDoUser? GetUser(Guid userId);
 *     ToDoUser? GetUserByTelegramUserId(long telegramUserId);
 *     void Add(ToDoUser user);
 * }
 * Создать класс InMemoryUserRepository, который реализует интерфейс IUserRepository. В качестве хранилища использовать List
 * Добавить использование IUserRepository в UserService. Получать IUserRepository нужно через конструктор
 */

namespace Otus_Annonumous_types_Tuple_Homework_7
{
    /// <summary>
    /// Класс хранения пользователей.
    /// </summary>
    public class InMemoryUserRepository : IUserRepository
    {
        public InMemoryUserRepository()
        {
            users = new List<ToDoUser>();
        }

        private readonly List<ToDoUser> users;          //Список пользователей.

        /// <summary>
        /// Получение пользователя по guid id.
        /// </summary>
        /// <param name="userId">Guid id пользователя.</param>
        /// <returns>Возвращает пользователя если он найден, null если нет.</returns>
        public ToDoUser? GetUser(Guid userId)
        {
            foreach (var user in users)
                if (user.UserId == userId)
                    return user;

            return null;
        }

        /// <summary>
        /// Получение пользователя по telegram id.
        /// </summary>
        /// <param name="userId">Telegram id пользователя.</param>
        /// <returns>Возвращает пользователя если он найден, null если нет.</returns>
        public ToDoUser? GetUserByTelegramUserId(long telegramUserId)
        {
            foreach (var user in users)
                if (user.TelegramUserId == telegramUserId)
                    return user;

            return null;
        }

        /// <summary>
        /// Добавление пользователя в репозиторий.
        /// </summary>
        /// <param name="user">Объект пользователя.</param>
        public void Add(ToDoUser user)
        {
            users.Add(user);
        }
    }
}
