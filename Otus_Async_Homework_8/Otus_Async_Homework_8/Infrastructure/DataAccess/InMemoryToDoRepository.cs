using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Async_Homework_8
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
        /// <returns>Список всех задач.</returns>
        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            return tasks.Where(x => x.User.UserId == userId).ToList();
        }

        /// <summary>
        /// Метод получения всех активных задач пользователя.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <returns>Список задач.</returns>
        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            return tasks.Where(x => x.User.UserId == userId && x.State == ToDoItemState.Active).ToList();
        }

        /// <summary>
        /// Метод добавления задачи в список задач.
        /// </summary>
        /// <param name="item">Задача.</param>
        public void Add(ToDoItem item)
        {
            tasks.Add(item);
        }

        /// <summary>
        /// Обновление задачи.
        /// </summary>
        /// <param name="item">Задача.</param>
        public void Update(ToDoItem item)
        {
            int index = tasks.FindIndex(x => x.Id == item.Id);
            tasks[index] = item;
        }

        /// <summary>
        /// Удаление задачи.
        /// </summary>
        /// <param name="id">Id задачи.</param>
        public void Delete(Guid id)
        {
            int index = tasks.FindIndex(x => x.Id == id);
            tasks.RemoveAt(index);
        }

        /// <summary>
        /// Метод проверки задачи у пользователя.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <param name="name">Имя задачи.</param>
        /// <returns>true если задача есть у текущего пользователя, false если нет.</returns>
        public bool ExistsByName(Guid userId, string name)
        {
            int items = tasks.Where(x => x.User.UserId == userId && x.Name == name).ToList().Count();

            if (items == 0)
                return true;

            return false;
        }

        /// <summary>
        /// Метод получения количества активных задач пользователя.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <returns>Количество задач.</returns>
        public int CountActive(Guid userId)
        {
            return tasks.Where(x => x.User.UserId == userId).ToList().Count;
        }

        /// <summary>
        /// Метод поиска задач по указанному предикату.
        /// </summary>
        /// <param name="userId">id пользователя</param>
        /// <param name="predicate">Параметр поиска.</param>
        /// <returns>Найденный список.</returns>
        public IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate)
        {
            return tasks.Where(x => x.User.UserId == userId).Where(predicate).ToList();
        }
    }
}
