using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Scenario_Homework_11
{
    public interface IToDoReportService
    {
        /// <summary>
        /// Получение информации о задачах пользователя.
        /// </summary>
        /// <param name="userId">ID пользователя.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        /// <returns>Все задачи, выполненные задачи, активные задачи, время обращения к методу.</returns>
        Task<(int total, int completed, int active, DateTime generatedAt)> GetUserStats(Guid userId, CancellationToken ct);
    }
}
