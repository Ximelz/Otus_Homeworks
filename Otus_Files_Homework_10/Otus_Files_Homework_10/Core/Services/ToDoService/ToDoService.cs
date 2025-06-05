using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Files_Homework_10
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
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Список активных задач.</returns>
        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            return await toDoRep.GetActiveByUserId(userId, ct);
        }

        /// <summary>
        /// Получение списка всех задач пользователя.
        /// </summary>
        /// <param name="userId">id пользователя.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Список задач указанного пользователя.</returns>
        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            return await toDoRep.GetAllByUserId(userId, ct);
        }

        /// <summary>
        /// Добавление задачи.
        /// </summary>
        /// <param name="user">Пользователь, добавивший задачу.</param>
        /// <param name="name">Имя задачи.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Добавленная задача.</returns>
        public async Task<ToDoItem> Add(ToDoUser user, string name, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            if (await toDoRep.CountActive(user.UserId, ct) >= maxTasks)
                throw new TaskCountLimitException(maxTasks);

            if (name.Length > maxLengthNameTask)
                throw new TaskLengthLimitException(name.Length, maxLengthNameTask);

            if (await toDoRep.ExistsByName(user.UserId, name, ct))
                throw new DuplicateTaskException(name);

            ToDoItem newItem = new ToDoItem(name, user);
            await toDoRep.Add(newItem, ct);
            return newItem; 
        }

        /// <summary>
        /// Отметка выполнения задачи.
        /// </summary>
        /// <param name="id">id выполненной задачи.</param>
        /// <param name="user">Пользователь, который выполнил задачу.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        public async Task MarkCompleted(Guid id, ToDoUser user, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            ToDoItem task = (await GetAllByUserId(user.UserId, ct)).Single(x => x.Id == id);

            if (task.State == ToDoItemState.Completed)
                throw new CompleteTaskException(id);

            task.State = ToDoItemState.Completed;
            task.StateChangedAt = DateTime.Now;
            await toDoRep.Update(task, ct);
        }

        /// <summary>
        /// Удаление задачи.
        /// </summary>
        /// <param name="id">id задачи на удаление.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        public async Task Delete(Guid id, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            await toDoRep.Delete(id, ct);
        }

        /// <summary>
        /// Поиск задач с указанным префиксом.
        /// </summary>
        /// <param name="user">Пользователь, чьи задачи ищем.</param>
        /// <param name="namePrefix">Префикс задач.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Список задач с префиксом.</returns>
        public async Task<IReadOnlyList<ToDoItem>> Find(ToDoUser user, string namePrefix, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            return await toDoRep.Find(user.UserId, x => x.Name.StartsWith(namePrefix), ct);
        }
    }
}
