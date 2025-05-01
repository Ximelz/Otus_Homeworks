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
    /// Перечесление статусов задачи.
    /// </summary>
    public enum ToDoItemState
    {
        Active,
        Completed
    }
}
