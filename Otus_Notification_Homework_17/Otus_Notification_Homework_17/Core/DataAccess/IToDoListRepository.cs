using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Notification_Homework_17
{
    public interface IToDoListRepository
    {
        /// <summary>
        /// Метод получения списка.
        /// </summary>
        /// <param name="id">Id задачи для получения.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Списсок. Если списка нет, то возвращает null.</returns>
        Task<ToDoList?> Get(Guid id, CancellationToken ct);

        /// <summary>
        /// Получение списка списков по id пользователя.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Список списков пользователя.</returns>
        Task<IReadOnlyList<ToDoList>> GetByUserId(Guid userId, CancellationToken ct);

        /// <summary>
        /// Добавление списка.
        /// </summary>
        /// <param name="list">Список для добавления.</param>
        /// <param name="ct">Токен отмены.</param>
        Task Add(ToDoList list, CancellationToken ct);

        /// <summary>
        /// Удаление списка по id.
        /// </summary>
        /// <param name="id">Id списка для удаления.</param>
        /// <param name="ct">Токен отмены.</param>
        Task Delete(Guid id, CancellationToken ct);

        /// <summary>
        /// Проверяет, если ли у пользователя список с таким именем.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <param name="name">Наименование задачи.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>true если список с таким именем есть, false если нет.</returns>
        Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct);
    }
}
