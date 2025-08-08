using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB.Mapping;

namespace Otus_Notification_Homework_17
{
    [Table("ToDoUser")]
    public class ToDoUserModel
    {
        [Column("UserId"), PrimaryKey]
        public Guid UserId { get; set; }                //id пользователя. Получает значение при инициализации, после чего не может быть изменен.

        [Column("TelegramUserId"), NotNull]
        public long TelegramUserId { get; set; }        //id текущего пользователя в telegram. Получает значение при инициализации, после чего не может быть изменен.

        [Column("TelegramUserName"), NotNull]
        public string TelegramUserName { get; set; }    //Имя текущего пользователя в telegram. Получает значение при инициализации, после чего не может быть изменен.

        [Column("RegisteredAt"), NotNull]
        public DateTime RegisteredAt { get; set; }      //Дата регистрации пользователя. Получает значение при инициализации, после чего не может быть изменен.
    }
}
