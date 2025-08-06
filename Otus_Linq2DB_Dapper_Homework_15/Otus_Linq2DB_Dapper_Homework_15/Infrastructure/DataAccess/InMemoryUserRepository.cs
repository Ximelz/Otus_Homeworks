using System.Security.Authentication;

namespace Otus_Linq2DB_Dapper_Homework_15
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
            ct.ThrowIfCancellationRequested();

            foreach (var user in users)
                if (user.UserId == userId)
                    return Task.FromResult(user);

            throw new AuthenticationException("Вы не авторизированны в программе! Используйте команду \"/start\" для авторизации.");
        }

        /// <summary>
        /// Получение пользователя по telegram id.
        /// </summary>
        /// <param name="telegramUserId">Telegram id пользователя.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Возвращает пользователя если он найден, null если нет.</returns>
        public Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            foreach (var user in users)
                if (user.TelegramUserId == telegramUserId)
                    return Task.FromResult(user);

            throw new AuthenticationException("Вы не авторизированны в программе! Используйте команду \"/start\" для авторизации.");
        }

        /// <summary>
        /// Добавление пользователя в репозиторий.
        /// </summary>
        /// <param name="user">Объект пользователя.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        public Task Add(ToDoUser user, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            users.Add(user);

            return Task.CompletedTask;
        }
    }
}
