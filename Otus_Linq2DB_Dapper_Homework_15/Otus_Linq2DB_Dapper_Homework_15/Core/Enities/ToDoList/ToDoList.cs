using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Linq2DB_Dapper_Homework_15
{
    /// <summary>
    /// Список задач.
    /// </summary>
    public class ToDoList
    {
        public ToDoList() { }
        /// <summary>
        /// Конструктор списка задач.
        /// </summary>
        /// <param name="name">Наименование списка.</param>
        /// <param name="user">Владелец списка.</param>
        public ToDoList(string name, ToDoUser user)
        {
            this.Name = name;
            this.User = user;
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }
        public Guid Id { get; set; }                //Id списка.
        public string Name { get; set; }            //Наименование списка.
        public ToDoUser User { get; set; }          //Пользователь, которому принадлежит список.
        public DateTime CreatedAt { get; set; }     //Время создания списка.
    }
}
