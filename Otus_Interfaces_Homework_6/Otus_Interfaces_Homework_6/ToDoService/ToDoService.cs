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
        public ToDoService(int maxTasks, int maxLengthNameTask)
        {
            tasks = new List<ToDoItem> ();
            this.maxTasks = maxTasks;
            this.maxLengthNameTask = maxLengthNameTask;
        }
        private readonly List<ToDoItem> tasks; //Список всех задач.
        private readonly int maxTasks;
        private readonly int maxLengthNameTask;

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

        //// <summary>
        /// Получение списка всех задач пользователя.
        /// </summary>
        /// <param name="userId">id пользователя.</param>
        /// <returns>Список задач указанного пользователя.</returns>
        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            List<ToDoItem> resultTasks = new List<ToDoItem>();

            foreach (var task in tasks)
                if (userId == task.User.UserId)
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
            if (tasks.Count >= maxTasks)
                throw new UserException($"Превышено максимальное количество задач равное \"{maxTasks}\"");

            if (name.Length > maxLengthNameTask)
                throw new UserException($"Длина задачи \"{name.Length}\" превышает максимально допустимое значение \"{maxLengthNameTask}\"");

            if (!DublicateCheck(name))
                throw new UserException($"Задача \"{name}\" уже существует!");

            ToDoItem newItem = new ToDoItem(name, user);
            tasks.Add(newItem);
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
                {
                    task.State = ToDoItemState.Completed;
                    task.StateChangedAt = DateTime.Now;
                }
        }

        /// <summary>
        /// Удаление задачи.
        /// </summary>
        /// <param name="id">id задачи на удаление.</param>
        public void Delete(Guid id)
        {
            int tasksCount = tasks.Count;
            for (int i = 0; i < tasksCount; i++)
            {
                if (tasks[i].Id == id)
                {
                    tasks.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Проверка на дубль задачи.
        /// </summary>
        /// <param name="name">Имя новой задачи.</param>
        /// <returns>true если такой задачи нет, false если найден дубль.</returns>
        private bool DublicateCheck(string name)
        {
            foreach (ToDoItem toDoItem in tasks)
                if (toDoItem.Name == name)
                    return false;

            return true;
        }
    }
}
