using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Otus_Concurrent_Homework_12
{
    public interface IScenario
    {
        /// <summary>
        /// Определение возможности выполнения данного сценария.
        /// </summary>
        /// <param name="scenario">Тип сценария.</param>
        /// <returns>Возвращает true если сценарий может выполниться, false если нет.</returns>
        bool CanHandle(ScenarioType scenario);

        /// <summary>
        /// Логика работы сценария.
        /// </summary>
        /// <param name="bot">Сессия пользователя из Telegram.</param>
        /// <param name="context">Данные с информацией о сценарии.</param>
        /// <param name="update">Информация о сообщении из telegram.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns>Результат выполнения сценария.</returns>
        Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct);
    }

}
