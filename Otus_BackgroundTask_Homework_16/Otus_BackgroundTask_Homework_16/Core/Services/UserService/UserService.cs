namespace Otus_BackgroundTask_Homework_16
{
    /// <summary>
    /// Класс для взаимодействия с пользователями.
    /// </summary>
    public class UserService : IUserService
    {
        public UserService(IUserRepository userRep)
        {
            this.userRep = userRep;
        }

        private readonly IUserRepository userRep;              //Ссылка на пользовательский репозиторий

        /// <summary>
        /// Регистрация нового пользователя.
        /// </summary>
        /// <param name="telegramUserId">id пользователя из telegram</param>
        /// <param name="telegramUserName">Имя пользователя из telegram</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Объект нового пользователя.</returns>
        public async Task<ToDoUser> RegisterUser(long telegramUserId, string telegramUserName, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            ToDoUser? user = await userRep.GetUserByTelegramUserId(telegramUserId, ct);

            if (user == null)
            {
                user = new ToDoUser(telegramUserId, telegramUserName);
                await userRep.Add(user, ct);
            }

            return user;
        }

        /// <summary>
        /// Получения уже авторизированного пользователя.
        /// </summary>
        /// <param name="telegramUserId">id пользователя из telegram</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Объект пользователя.</returns>
        public async Task<ToDoUser?> GetUser(long telegramUserId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            return await userRep.GetUserByTelegramUserId(telegramUserId, ct);
        }
    }
}
