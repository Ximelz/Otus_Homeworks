using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Otus_BackgroundTask_Homework_16
{
    [Table("ToDoList")]
    public class ToDoListModel
    {
        [Column("Id"), PrimaryKey]
        public Guid Id { get; set; }                //Id списка.

        [Column("Name"), NotNull]
        public string Name { get; set; }            //Наименование списка.

        [Column("CreatedAt"), NotNull]
        public DateTime CreatedAt { get; set; }     //Время создания списка.

        [Column("UserId"), NotNull]
        public Guid UserId { get; set; }            //ID пользователя, создавшего список.

        [Association(ThisKey = nameof(UserId), OtherKey = nameof(ToDoUserModel.UserId))]
        public ToDoUserModel User { get; set; }          //Пользователь, которому принадлежит список.
    }
}
