using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Files_Homework_10
{
    /// <summary>
    /// Класс объекта задачи.
    /// </summary>
    public class ToDoItem
    {
        /// <summary>
        /// Конструктор объекта задачи.
        /// </summary>
        /// <param name="Name">Наименование задачи.</param>
        /// <param name="User">Пользователь, создавший задачу.</param>
        public ToDoItem(string Name, ToDoUser User)
        {            
            State = ToDoItemState.Active;
            CreatedAt = DateTime.Now;
            Id = Guid.NewGuid();
            this.Name = Name;
            this.User = User;
        }
        public Guid Id { get; init; }                           //Сгенерированный id задачи. Значение вычисляется 1 раз при создании объекта.
        public ToDoUser User { get; init; }                     //Пользователь, создавший задачу. Значение присваивается 1 раз при создании объекта.
        public string Name { get; init; }                       //Наименование задачи. Значение присваивается 1 раз при создании объекта.
        public DateTime CreatedAt { get; init; }                //Дата создания задачи. Значение вычисляется 1 раз при создании объекта.
        public ToDoItemState State { get; set; }                //Свойство, при котором меняется статус задачи и дата его изменения.
        public DateTime? StateChangedAt { get; set; }           //Дата изменения статуса задачи.
    }
}
