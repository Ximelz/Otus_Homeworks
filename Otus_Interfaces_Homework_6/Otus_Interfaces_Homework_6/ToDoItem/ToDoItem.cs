using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 5. Добавление класса ToDoItem
 *       5.1 Добавить enum ToDoItemState с двумя значениями
 *           5.1.1 Active
 *           5.1.2 Completed
 *       5.2 Добавить класс ToDoItem
 *           5.2.1 Свойства
 *           5.2.2 Guid Id
 *           5.2.3 User User
 *           5.2.4 string Name
 *           5.2.5 DateTime CreatedAt
 *           5.2.6 ToDoItemState State
 *           5.2.7 DateTime? StateChangedAt - обновляется при изменении State
 *       5.3 Добавить использование класса ToDoItem вместо хранения только имени задачи
 */

namespace Otus_Interfaces_Homework_6
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
        public ToDoItem(string Name, ConsoleUser User)
        {            
            State = ToDoItemState.Active;
            CreatedAt = DateTime.Now;
            Id = Guid.NewGuid();
            this.Name = Name;
            this.User = User;
        }
        private ToDoItemState _state;                           //Скрытое поле для хранения информации о статусе задачи.
        public Guid Id { get; init; }                           //Сгенерированный id задачи. Значение вычисляется 1 раз при создании объекта.
        public ConsoleUser User { get; init; }                  //Пользователь, создавший задачу. Значение присваивается 1 раз при создании объекта.
        public string Name { get; init; }                       //Наименование задачи. Значение присваивается 1 раз при создании объекта.
        public DateTime CreatedAt { get; init; }                //Дата создания задачи. Значение вычисляется 1 раз при создании объекта.
        public ToDoItemState State                              //Свойство, при котором меняется статус задачи и дата его изменения.
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                StateChangedAt = DateTime.Now;
            }
        }
        public DateTime? StateChangedAt { get; private set; }   //Дата изменения статуса задачи. Вычисляется каждый раз при изменении статуса задачи.
    }
}
