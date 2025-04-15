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
    public interface IToDoReportService
    {
        (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId);         //Получение информации о задачах пользователя.
    }
}
