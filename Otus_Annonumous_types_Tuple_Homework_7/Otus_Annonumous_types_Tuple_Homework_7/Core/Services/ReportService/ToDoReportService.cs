using Otus_Interfaces_Homework_6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Кортежи. Добавление команды /report
 * Добавить метод IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId); в интерфейс IToDoRepository. Метод должен возвращать все задачи пользователя
 * Добавить интерфейс IToDoReportService
 * interface IToDoReportService
 * {
 *     (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId);
 * }
 * Создать класс ToDoReportService, который реализует интерфейс IToDoReportService.
 * Добавить обработку новой команды /report. Нужно использовать IToDoReportService
 * Пример вывода: Статистика по задачам на 01.01.2025 00:00:00. Всего: 10; Завершенных: 7; Активных: 3;
 */

namespace Otus_Annonumous_types_Tuple_Homework_7
{
    public class ToDoReportService : IToDoReportService
    {
        public ToDoReportService(IToDoService toDoSer)
        {
            this.toDoSer = toDoSer;
        }

        private readonly IToDoService toDoSer;           //Репозиторий задач.

        /// <summary>
        /// Метод получения информации о задачах пользователя.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <returns>Все задачи, выполненные задачи, активные задачи, время обращения к методу.</returns>
        public (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId)
        {
            IReadOnlyList<ToDoItem> tasks = toDoSer.GetAllByUserId(userId);
            int totalTasks = tasks.Count;
            int activeTasks = tasks.Where(x => x.State == ToDoItemState.Active).ToList().Count();

            return (totalTasks, totalTasks - activeTasks, activeTasks, DateTime.Now);
        }
    }
}
