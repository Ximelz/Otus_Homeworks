using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Async_Homework_8
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
        /// <returns>Объект нового пользователя.</returns>
        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            ToDoUser user = new ToDoUser(telegramUserId, telegramUserName);
            userRep.Add(user);
            return user;
        }

        /// <summary>
        /// Получения уже авторизированного пользователя.
        /// </summary>
        /// <param name="telegramUserId">id пользователя из telegram</param>
        /// <returns>Объект пользователя.</returns>
        public ToDoUser? GetUser(long telegramUserId)
        {
            ToDoUser user = userRep.GetUserByTelegramUserId(telegramUserId);
            if (user != null)
                return user;

            return null;
        }
    }
}
