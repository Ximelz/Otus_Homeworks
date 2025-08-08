using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Notification_Homework_17
{
    public interface IScenarioContextRepository
    {
        /// <summary>
        /// Получение объекта данных сценария пользователя.
        /// </summary>
        /// <param name="userId">Id telegram пользователя.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Объект данных сценария.</returns>
        Task<ScenarioContext?> GetContext(long userId, CancellationToken ct);

        /// <summary>
        /// Добавление объекта данных сценария пользователя для сохранения.
        /// </summary>
        /// <param name="userId">Id telegram пользователя.</param>
        /// <param name="context">Объект данных сценария.</param>
        /// <param name="ct">Токен отмены.</param>
        Task SetContext(long userId, ScenarioContext context, CancellationToken ct);

        /// <summary>
        /// Сброс данных сценария пользователя.
        /// </summary>
        /// <param name="userId">Id telegram пользователя.</param>
        /// <param name="ct">Токен отмены.</param>
        Task ResetContext(long userId, CancellationToken ct);

        /// <summary>
        /// Получение списка всех активных сценариев пользователей.
        /// </summary>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Список сценариев.</returns>
        Task<IReadOnlyList<ScenarioContext>> GetContexts(CancellationToken ct);
    }
}
