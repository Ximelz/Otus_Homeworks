using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Notification_Homework_17
{
    /// <summary>
    /// Класс консольного пользователя.
    /// </summary>
    public class ToDoUser
    {
        public ToDoUser() { }
        public ToDoUser(long TelegramUserId, string TelegramUserName)
        {
            UserId = Guid.NewGuid();
            RegisteredAt = DateTime.Now;
            this.TelegramUserId = TelegramUserId;
            this.TelegramUserName = TelegramUserName;
        }
        public Guid UserId { get; set; }                   //id пользователя. Получает значение при инициализации, после чего не может быть изменен.
        public long TelegramUserId { get; set; }           //id текущего пользователя в telegram. Получает значение при инициализации, после чего не может быть изменен.
        public string TelegramUserName { get; set; }       //Имя текущего пользователя в telegram. Получает значение при инициализации, после чего не может быть изменен.
        public DateTime RegisteredAt { get; set; }         //Дата регистрации пользователя. Получает значение при инициализации, после чего не может быть изменен.
    }
}
