using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Otus_BackgroundTask_Homework_16
{
    public class ResetScenarioBackgroundTask : BackgroundTask
    {
        public ResetScenarioBackgroundTask(TimeSpan resetScenarioTimeout, IScenarioContextRepository contextRep, ITelegramBotClient botClient) : base(TimeSpan.FromHours(1), nameof(ResetScenarioBackgroundTask))
        {
            this.resetScenarioTimeout = resetScenarioTimeout;
            this.botClient = botClient;
            this.contextRep = contextRep;
        }

        private readonly TimeSpan resetScenarioTimeout;
        private readonly IScenarioContextRepository contextRep;
        private readonly ITelegramBotClient botClient;
        protected override async Task Execute(CancellationToken ct)
        {
            DateTime now = DateTime.UtcNow;
            var scenaries = (await contextRep.GetContexts(ct)).Where(x => x.CreatedAt + resetScenarioTimeout > now);
            ReplyMarkup keyboard = KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Default);
            string message = $"Сценарий отменен, так как не поступил ответ в течение {resetScenarioTimeout}";
            foreach (ScenarioContext scenario in scenaries)
            {
                await contextRep.ResetContext(scenario.UserId, ct);
                await botClient.SendMessage(chatId: scenario.UserId, message, replyMarkup: keyboard, cancellationToken: ct);
            }
        }
    }
}
