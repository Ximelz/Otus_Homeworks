namespace Otus_Linq_Homework_13
{
    public class InMemoryToDoRepository : IToDoRepository
    {
        public InMemoryToDoRepository()
        {
            tasks = new List<ToDoItem>();
        }

        private readonly List<ToDoItem> tasks;                  //Список всех задач.

        /// <summary>
        /// Метод получения всех задач пользователя.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Список всех задач.</returns>
        public Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            return Task.FromResult((IReadOnlyList<ToDoItem>)tasks.Where(x => x.User.UserId == userId).ToList());
        }

        /// <summary>
        /// Метод получения всех активных задач пользователя.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Список задач.</returns>
        public Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            return Task.FromResult((IReadOnlyList<ToDoItem>)tasks.Where(x => x.User.UserId == userId && x.State == ToDoItemState.Active).ToList());
        }

        /// <summary>
        /// Метод добавления задачи в список задач.
        /// </summary>
        /// <param name="item">Задача.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        public Task Add(ToDoItem item, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            tasks.Add(item);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Обновление задачи.
        /// </summary>
        /// <param name="item">Задача.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        public Task Update(ToDoItem item, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            int index = tasks.FindIndex(x => x.Id == item.Id);
            tasks[index] = item;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Удаление задачи.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <param name="id">Id задачи.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        public Task Delete(Guid id, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            int index = tasks.FindIndex(x => x.Id == id);

            tasks.RemoveAt(index);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Метод проверки задачи у пользователя.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <param name="name">Имя задачи.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>true если задача есть у текущего пользователя, false если нет.</returns>
        public Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            int items = tasks.Where(x => x.User.UserId == userId && x.Name == name).ToList().Count();

            if (items == 0)
                return Task.FromResult(true);

            return Task.FromResult(false);
        }

        /// <summary>
        /// Метод получения количества активных задач пользователя.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Количество задач.</returns>
        public Task<int> CountActive(Guid userId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            return Task.FromResult(tasks.Where(x => x.User.UserId == userId).ToList().Count);
        }

        /// <summary>
        /// Метод поиска задач по указанному предикату.
        /// </summary>
        /// <param name="userId">id пользователя</param>
        /// <param name="predicate">Параметр поиска.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Найденный список.</returns>
        public Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            return Task.FromResult((IReadOnlyList<ToDoItem>)tasks.Where(x => x.User.UserId == userId).Where(predicate).ToList());
        }

        public async Task<ToDoItem?> Get(Guid toDoItemId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            return tasks.Where(x => x.Id == toDoItemId).FirstOrDefault();
        }
    }
}
