using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Notification_Homework_17
{
    public interface IToDoListService
    {
        /// <summary>
        /// Метод добавления списка задач.
        /// </summary>
        /// <param name="user">Пользователь, который добавляет список.</param>
        /// <param name="name">Наименование списка.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Добавленный список.</returns>
        Task<ToDoList> Add(ToDoUser user, string name, CancellationToken ct);

        /// <summary>
        /// Метод получения списка.
        /// </summary>
        /// <param name="id">Id списка.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Найденный список. Если списка нет, то возвращает null.</returns>
        Task<ToDoList?> Get(Guid id, CancellationToken ct);

        /// <summary>
        /// Метод удаления списка.
        /// </summary>
        /// <param name="id">Id списка.</param>
        /// <param name="ct">Токен отмены.</param>
        Task Delete(Guid id, CancellationToken ct);

        /// <summary>
        /// Метод получения списка списков пользователя.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Список списков пользователя.</returns>
        Task<IReadOnlyList<ToDoList>> GetUserLists(Guid userId, CancellationToken ct);
    }
}
