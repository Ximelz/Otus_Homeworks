using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 7. Добавление класса сервиса ToDoService
 *       7.1 Добавить интерфейс IToDoService
 *           
 *           public interface IToDoService
 *           {
 *              //Возвращает ToDoItem для UserId со статусом Active
 *              IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
 *              ToDoItem Add(User user, string name);
 *              void MarkCompleted(Guid id);
 *              void Delete(Guid id);
 *           }
 *           
 *       7.2 Создать класс ToDoService, который реализует интерфейс IToDoService. Перенести в него логику обработки команд
 *       7.3 Добавить использование IToDoService в UpdateHandler. Получать IToDoService нужно через конструктор
 *       7.4 Изменить формат обработки команды /addtask. Нужно сразу передавать имя задачи в команде. Пример: /addtask Новая задача
 *       7.5 Изменить формат обработки команды /removetask. Нужно сразу передавать номер задачи в команде. Пример: /removetask 2
 */

namespace Otus_Interfaces_Homework_6
{
    /// <summary>
    /// Класс для взаимодействия с задачами.
    /// </summary>
    public class ToDoService : IToDoService
    {
        public ToDoService()
        {
            tasks = new List<ToDoItem> ();
            tasksCount = 0;
        }
        public List<ToDoItem> tasks { get; private set; }  //Список всех задач.
        public int tasksCount { get; private set; }        //Количество задач.

        /// <summary>
        /// Получение списка всех активных задач пользователя.
        /// </summary>
        /// <param name="userId">id пользователя.</param>
        /// <returns>Список активных задач.</returns>
        public IReadOnlyList<ToDoItem> GetActiveByUserID(Guid userId)
        {
            List<ToDoItem> resultTasks = new List<ToDoItem>();

            foreach (var task in tasks)
                if (userId == task.User.UserId)
                    if (task.State == ToDoItemState.Active)
                        resultTasks.Add(task);

            return resultTasks;
        }

        /// <summary>
        /// Добавление задачи.
        /// </summary>
        /// <param name="user">Пользователь, добавивший задачу.</param>
        /// <param name="name">Имя задачи.</param>
        /// <returns>Добавленная задача.</returns>
        public ToDoItem Add(ConsoleUser user, string name)
        {
            ToDoItem newItem = new ToDoItem(name, user);
            tasks.Add(newItem);
            tasksCount++;
            return newItem;
        }

        /// <summary>
        /// Отметка выполнения задачи.
        /// </summary>
        /// <param name="id">id выполненной задачи.</param>
        public void MarkCompleted(Guid id)
        {
            foreach (var task in tasks)
                if (task.Id == id)
                    task.State = ToDoItemState.Completed;
        }

        /// <summary>
        /// Удаление задачи.
        /// </summary>
        /// <param name="id">id задачи на удаление.</param>
        public void Delete(Guid id)
        {
            for (int i = 0; i < tasksCount; i++)
            {
                if (tasks[i].Id == id)
                {
                    tasks.RemoveAt(i);
                    tasksCount--;
                    break;
                }
            }
        }
    }
}
