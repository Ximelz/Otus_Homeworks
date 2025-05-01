using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Async_Homework_8
{
    public interface IToDoService
    {
        /// <summary>
        /// Метод получения всех активных задач пользователя.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <returns>Список активных задач пользователя.</returns>
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);

        /// <summary>
        /// Метод получения всех задач пользователя.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <returns>Список задач пользователя.</returns>
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);

        /// <summary>
        /// Метод добавления новой задачи.
        /// </summary>
        /// <param name="user">Пользователь, который добавляет задачу.</param>
        /// <param name="name">Наименование задачи.</param>
        /// <returns>Добавленная задача.</returns>
        ToDoItem Add(ToDoUser user, string name);

        /// <summary>
        /// Метод отметки задачи как выполненной.
        /// </summary>
        /// <param name="id">ID задачи.</param>
        /// <param name="user">Пользователь, который выполнил задачу.</param>
        void MarkCompleted(Guid id, ToDoUser user);

        /// <summary>
        /// Метод удаления задачи.
        /// </summary>
        /// <param name="id">ID задачи.</param>
        void Delete(Guid id);

        /// <summary>
        /// Метод поиска задач пользователя с указанным префиксом.
        /// </summary>
        /// <param name="user">Пользователь, который ищет свои задачи.</param>
        /// <param name="namePrefix">Префикс наименования задачи, по которому ищутся задачи.</param>
        /// <returns>Найденные задачи.</returns>
        IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix);
    }
}
