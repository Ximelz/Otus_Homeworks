using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 3. Изменение логики команды /start
 *       3.1 Не нужно запрашивать имя
 *       3.2 Добавить класс User
 *           3.2.1 Свойства
 *                 3.2.1.1 Guid UserId
 *                 3.2.1.2 long TelegramUserId
 *                 3.2.1.3 string TelegramUserName
 *                 3.2.1.4 DateTime RegisteredAt
 */

namespace Otus_Interfaces_Homework_6
{
    /// <summary>
    /// Класс консольного пользователя.
    /// </summary>
    public class ConsoleUser
    {
        public ConsoleUser(long TelegramUserId, string TelegramUserName)
        {
            UserId = Guid.NewGuid();
            RegisteredAt = DateTime.Now;
            this.TelegramUserId = TelegramUserId;
            this.TelegramUserName = TelegramUserName;
        }
        public Guid UserId { get; init; }                   //id пользователя. Получает значение при инициализации, после чего не может быть изменен.
        public long TelegramUserId { get; init; }           //id текущего пользователя в telegram. Получает значение при инициализации, после чего не может быть изменен.
        public string TelegramUserName { get; init; }       //Имя текущего пользователя в telegram. Получает значение при инициализации, после чего не может быть изменен.
        public DateTime RegisteredAt { get; init; }         //Дата регистрации пользователя. Получает значение при инициализации, после чего не может быть изменен.
    }
}
