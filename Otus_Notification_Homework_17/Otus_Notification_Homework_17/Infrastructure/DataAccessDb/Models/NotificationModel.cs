using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Notification_Homework_17
{
    [Table("Notification")]
    public class NotificationModel
    {
        [Column("Id"), PrimaryKey]
        public Guid Id { set; get; }                           //Id нотификации.

        [Column("Type"), NotNull]
        public string Type { set; get; }                       //Тип нотификации. Например: DeadLine_{ToDoItem.Id}, Today_{DateOnly.FromDateTime(DateTime.UtcNow)}.

        [Column("Text"), NotNull]
        public string Text { set; get; }                       //Текст, который будет отправлен.

        [Column("ScheduledAt"), NotNull]
        public DateTime ScheduledAt { set; get; }              //Запланированная дата отправки.

        [Column("IsNotified")]
        public bool IsNotified { set; get; }                   //Флаг отправки.

        [Column("NotifiedAt")]
        public DateTime? NotifiedAt { set; get; }              //Фактическая дата отправки.

        [Column("UserId"), NotNull]
        public Guid UserId { set; get; }                       //Id пользователя нотификации.


        [Association(ThisKey = nameof(UserId), OtherKey = nameof(ToDoUserModel.UserId))]
        public ToDoUserModel User { set; get; }                //Пользователь нотификации.
    }
}
