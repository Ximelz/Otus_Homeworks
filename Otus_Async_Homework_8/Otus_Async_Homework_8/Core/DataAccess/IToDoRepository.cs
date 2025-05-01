using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Async_Homework_8
{
    public interface IToDoRepository
    {
        /// <summary>
        /// Метод получения списка всех задач пользователя.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <returns>Список задач пользователя.</returns>
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);

        /// <summary>
        /// Метод получения списка активных задач пользователя.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <returns>Список активных задач пользователя.</returns>
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);

        /// <summary>
        /// Метод добавления задачи.
        /// </summary>
        /// <param name="item">Задача для добавления.</param>
        void Add(ToDoItem item);

        /// <summary>
        /// Метод обновления задачи.
        /// </summary>
        /// <param name="item">Задача для обновления.</param>
        void Update(ToDoItem item);

        /// <summary>
        /// Метод удаления задачи.
        /// </summary>
        /// <param name="id">ID задачи для удаления.</param>
        void Delete(Guid id);

        /// <summary>
        /// Метод проверки наличия у пользователя задачи.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <param name="name">Наименование задачи.</param>
        /// <returns>true если задача есть, false если такой задачи у пользователя нет.</returns>
        bool ExistsByName(Guid userId, string name);

        /// <summary>
        /// Метод получения количества задач пользователя.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <returns>Количество задач пользователя.</returns>
        int CountActive(Guid userId);

        /// <summary>
        /// Метод для поиска задач по указанному предикату.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <param name="predicate">Метод-фильтр для поиска.</param>
        /// <returns>Список найденных задач, удовлетворяющих заданному фильтру.</returns>
        IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate);
    }
}
