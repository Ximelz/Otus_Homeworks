using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Otus.ToDoList.ConsoleBot.Types;

namespace Otus_Async_Homework_8
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
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Возвращает пользователя если он найден, null если нет.</returns>
        public Task<ToDoUser?> GetUser(Guid userId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();
            
            foreach (var user in users)
                if (user.UserId == userId)
                    return Task.FromResult(user);

            return null;
        }

        /// <summary>
        /// Получение пользователя по telegram id.
        /// </summary>
        /// <param name="userId">Telegram id пользователя.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Возвращает пользователя если он найден, null если нет.</returns>
        public Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            foreach (var user in users)
                if (user.TelegramUserId == telegramUserId)
                    return Task.FromResult(user);

            return null;
        }

        /// <summary>
        /// Добавление пользователя в репозиторий.
        /// </summary>
        /// <param name="user">Объект пользователя.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        public Task Add(ToDoUser user, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            users.Add(user);

            return Task.CompletedTask;
        }
    }
}
