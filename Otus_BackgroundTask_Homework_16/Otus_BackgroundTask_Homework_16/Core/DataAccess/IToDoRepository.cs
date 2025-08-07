using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_BackgroundTask_Homework_16
{
    public interface IToDoRepository
    {
        /// <summary>
        /// Метод получения списка всех задач пользователя.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Список задач пользователя.</returns>
        Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct);

        /// <summary>
        /// Метод получения списка активных задач пользователя.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Список активных задач пользователя.</returns>
        Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct);

        /// <summary>
        /// Метод добавления задачи.
        /// </summary>
        /// <param name="item">Задача для добавления.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        Task Add(ToDoItem item, CancellationToken ct);

        /// <summary>
        /// Метод обновления задачи.
        /// </summary>
        /// <param name="item">Задача для обновления.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        Task Update(ToDoItem item, CancellationToken ct);

        /// <summary>
        /// Метод удаления задачи.
        /// </summary>
        /// <param name="id">ID задачи для удаления.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        Task Delete(Guid id, CancellationToken ct);

        /// <summary>
        /// Метод проверки наличия у пользователя задачи.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <param name="name">Наименование задачи.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>true если задача есть, false если такой задачи у пользователя нет.</returns>
        Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct);

        /// <summary>
        /// Метод получения количества задач пользователя.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Количество задач пользователя.</returns>
        Task<int> CountActive(Guid userId, CancellationToken ct);

        /// <summary>
        /// Метод для поиска задач по указанному предикату.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <param name="predicate">Метод-фильтр для поиска.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Список найденных задач, удовлетворяющих заданному фильтру.</returns>
        Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct);

        /// <summary>
        /// Метод получения задачи.
        /// </summary>
        /// <param name="toDoItemId">ID задачи.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Искомая задача.</returns>
        Task<ToDoItem?> Get(Guid toDoItemId, CancellationToken ct);
    }
}
