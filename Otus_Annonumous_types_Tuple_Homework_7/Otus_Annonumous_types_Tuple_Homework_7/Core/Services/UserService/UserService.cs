using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otus_Annonumous_types_Tuple_Homework_7;

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
