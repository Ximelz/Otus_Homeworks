using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 4. Добавление класса сервиса UserService
 *       4.1 Добавить интерфейс IUserService
 *       
 *           interface IUserService
 *           {
 *              User RegisterUser(long telegramUserId, string telegramUserName);
 *              User? GetUser(long telegramUserId);
 *           }
 *           
 *       4.2 Создать класс UserService, который реализует интерфейс IUserService. Заполнять telegramUserId и telegramUserName нужно из значений Update.Message.From
 *       4.3 Добавить использование IUserService в UpdateHandler. Получать IUserService нужно через конструктор
 *       4.4 При команде /start нужно вызвать метод IUserService.RegisterUser.
 *       4.5 Если пользователь не зарегистрирован, то ему доступны только команды /help /info
 */

namespace Otus_Interfaces_Homework_6
{
    /// <summary>
    /// Класс для взаимодействия с пользователями.
    /// </summary>
    public class UserService : IUserService
    {
        public UserService()
        {
            consoleUsers = new List<ConsoleUser>();
        }
        private readonly List<ConsoleUser> consoleUsers;     //Список зарегистрированных пользователей.

        /// <summary>
        /// Регистрация нового пользователя.
        /// </summary>
        /// <param name="telegramUserId">id пользователя из telegram</param>
        /// <param name="telegramUserName">Имя пользователя из telegram</param>
        /// <returns>Объект нового пользователя.</returns>
        public ConsoleUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            ConsoleUser user = new ConsoleUser(telegramUserId, telegramUserName);
            consoleUsers.Add(user);
            return user;
        }

        /// <summary>
        /// Получения уже авторизированного пользователя.
        /// </summary>
        /// <param name="telegramUserId">id пользователя из telegram</param>
        /// <returns>Объект пользователя.</returns>
        /// <exception cref="Exception">Если пользователя нет, то выдается исключение.</exception>
        public ConsoleUser? GetUser(long telegramUserId)
        {
            foreach (ConsoleUser user in consoleUsers)
                if (user.TelegramUserId == telegramUserId)
                    return user;

            return null;
        }
    }
}
