using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Concurrent_Homework_12
{
    public interface IToDoService
    {
        /// <summary>
        /// Метод получения всех активных задач пользователя.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Список активных задач пользователя.</returns>
        Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct);

        /// <summary>
        /// Метод получения всех задач пользователя.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Список задач пользователя.</returns>
        Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct);

        /// <summary>
        /// Метод добавления новой задачи.
        /// </summary>
        /// <param name="user">Пользователь, который добавляет задачу.</param>
        /// <param name="name">Наименование задачи.</param>
        /// <param name="deadLine">Крайний срок выполнения задачи.</param>
        /// <param name="list">Список, которому принадлежит задача..</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Добавленная задача.</returns>
        Task<ToDoItem> Add(ToDoUser user, string name, DateTime deadLine, ToDoList? list, CancellationToken ct);

        /// <summary>
        /// Метод отметки задачи как выполненной.
        /// </summary>
        /// <param name="id">ID задачи.</param>
        /// <param name="user">Пользователь, который выполнил задачу.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        Task MarkCompleted(Guid id, ToDoUser user, CancellationToken ct);

        /// <summary>
        /// Метод удаления задачи.
        /// </summary>
        /// <param name="id">ID задачи.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        Task Delete(Guid id, CancellationToken ct);

        /// <summary>
        /// Метод поиска задач пользователя с указанным префиксом.
        /// </summary>
        /// <param name="user">Пользователь, который ищет свои задачи.</param>
        /// <param name="namePrefix">Префикс наименования задачи, по которому ищутся задачи.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Найденные задачи.</returns>
        Task<IReadOnlyList<ToDoItem>> Find(ToDoUser user, string namePrefix, CancellationToken ct);

        /// <summary>
        /// Метод получения всех задач из списка пользователя.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <param name="listId">Id списка.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Список задач из списка.</returns>
        Task<IReadOnlyList<ToDoItem>> GetByUserIdAndList(Guid userId, Guid? listId, CancellationToken ct);
    }
}
