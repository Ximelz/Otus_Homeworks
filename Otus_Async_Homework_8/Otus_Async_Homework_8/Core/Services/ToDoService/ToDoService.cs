using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Async_Homework_8
{
    /// <summary>
    /// Класс для взаимодействия с задачами.
    /// </summary>
    public class ToDoService : IToDoService
    {
        public ToDoService(int maxTasks, int maxLengthNameTask, IToDoRepository toDoRep)
        {
            this.toDoRep = toDoRep;
            this.maxTasks = maxTasks;
            this.maxLengthNameTask = maxLengthNameTask;
        }

        private readonly int maxTasks;
        private readonly int maxLengthNameTask;
        private readonly IToDoRepository toDoRep;

        /// <summary>
        /// Получение списка всех активных задач пользователя.
        /// </summary>
        /// <param name="userId">id пользователя.</param>
        /// <returns>Список активных задач.</returns>
        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            return toDoRep.GetActiveByUserId(userId);
        }

        /// <summary>
        /// Получение списка всех задач пользователя.
        /// </summary>
        /// <param name="userId">id пользователя.</param>
        /// <returns>Список задач указанного пользователя.</returns>
        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            return toDoRep.GetAllByUserId(userId);
        }

        /// <summary>
        /// Добавление задачи.
        /// </summary>
        /// <param name="user">Пользователь, добавивший задачу.</param>
        /// <param name="name">Имя задачи.</param>
        /// <returns>Добавленная задача.</returns>
        public ToDoItem Add(ToDoUser user, string name)
        {
            if (toDoRep.CountActive(user.UserId) >= maxTasks)
                throw new TaskCountLimitException(maxTasks);

            if (name.Length > maxLengthNameTask)
                throw new TaskLengthLimitException(name.Length, maxLengthNameTask);

            if (!DublicateCheck(name, user))
                throw new DuplicateTaskException(name);

            ToDoItem newItem = new ToDoItem(name, user);
            toDoRep.Add(newItem);
            return newItem; 
        }

        /// <summary>
        /// Отметка выполнения задачи.
        /// </summary>
        /// <param name="id">id выполненной задачи.</param>
        /// <param name="user">Пользователь, который выполнил задачу.</param>
        public void MarkCompleted(Guid id, ToDoUser user)
        {
            IReadOnlyList<ToDoItem> tasks = GetAllByUserId(user.UserId).Where(x => x.Id == id).ToList();

            if (tasks.Count > 1)
                throw new ArgumentException($"В базе несколько задач имеют id \"{id}\"");

            if (tasks.Count < 1)
                throw new ArgumentException($"Задачи с таким id \"{id}\" нет в базе.");

            tasks[0].State = ToDoItemState.Completed;
            tasks[0].StateChangedAt = DateTime.Now;
            toDoRep.Update(tasks[0]);
        }

        /// <summary>
        /// Удаление задачи.
        /// </summary>
        /// <param name="id">id задачи на удаление.</param>
        public void Delete(Guid id)
        {
            toDoRep.Delete(id);
        }

        /// <summary>
        /// Поиск задач с указанным префиксом.
        /// </summary>
        /// <param name="user">Пользователь, чьи задачи ищем.</param>
        /// <param name="namePrefix">Префикс задач.</param>
        /// <returns>Список задач с префиксом.</returns>
        public IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix)
        {
            return toDoRep.Find(user.UserId, x => x.Name.Substring(0, namePrefix.Length) == namePrefix);
        }

        /// <summary>
        /// Проверка на дубль задачи.
        /// </summary>
        /// <param name="name">Имя новой задачи.</param>
        /// <param name="user">Пользователь, который пытается добавить задачу.</param>
        /// <returns>true если такой задачи нет, false если найден дубль.</returns>
        private bool DublicateCheck(string name, ToDoUser user)
        {
            return toDoRep.ExistsByName(user.UserId, name);
        }
    }
}
