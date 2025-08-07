using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_BackgroundTask_Homework_16
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
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Все задачи, выполненные задачи, активные задачи, время обращения к методу.</returns>
        public async Task<(int total, int completed, int active, DateTime generatedAt)> GetUserStats(Guid userId, CancellationToken ct)
        {
            IReadOnlyList<ToDoItem> tasks = await toDoSer.GetAllByUserId(userId, ct);
            int totalTasks = tasks.Count;
            int activeTasks = tasks.Where(x => x.State == ToDoItemState.Active).ToList().Count();

            return (totalTasks, totalTasks - activeTasks, activeTasks, DateTime.Now);
        }
    }
}
