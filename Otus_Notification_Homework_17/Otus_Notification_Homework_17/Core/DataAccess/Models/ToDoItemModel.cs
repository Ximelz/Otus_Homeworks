using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Otus_Notification_Homework_17
{
    [Table("ToDoItem")]
    public class ToDoItemModel
    {
        [Column("Id"), PrimaryKey]
        public Guid Id { get; set; }                           //Сгенерированный id задачи. Значение вычисляется 1 раз при создании объекта.

        [Column("Name"), NotNull]
        public string Name { get; set; }                       //Наименование задачи. Значение присваивается 1 раз при создании объекта.

        [Column("CreatedAt"), NotNull]
        public DateTime CreatedAt { get; set; }                //Дата создания задачи. Значение вычисляется 1 раз при создании объекта.

        [Column("State"), NotNull]
        public ToDoItemState State { get; set; }                //Свойство, при котором меняется статус задачи и дата его изменения.

        [Column("StateChangedAt")]
        public DateTime? StateChangedAt { get; set; }           //Дата изменения статуса задачи.

        [Column("DeadLine")]
        public DateTime? DeadLine { get; set; }                 //Крайняя дата выполнений задачи.

        [Column("UserId")]
        public Guid UserId { get; set; }                        //ID пользователя, который создал задачу.

        [Column("ListId")]
        public Guid? ListId { get; set; }                       //ID списка, в который входит задача.


        [Association(ThisKey = nameof(ListId), OtherKey = nameof(ToDoListModel.Id))]
        public ToDoListModel? List { get; set; }                     //Принадлежность задачи определенному списку.

        [Association(ThisKey = nameof(UserId), OtherKey = nameof(ToDoUserModel.UserId))]
        public ToDoUserModel User { get; set; }                      //Пользователь, создавший задачу. Значение присваивается 1 раз при создании объекта.
    }
}
