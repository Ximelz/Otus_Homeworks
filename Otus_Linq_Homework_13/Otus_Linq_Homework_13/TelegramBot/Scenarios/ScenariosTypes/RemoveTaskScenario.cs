using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Otus_Linq_Homework_13
{
    public class RemoveTaskScenario : IScenario
    {
        public RemoveTaskScenario(IToDoService toDoService, IUserService userService)
        {
            this.toDoService = toDoService;
            this.userService = userService;
        }

        private readonly IToDoService toDoService;
        private readonly IUserService userService;
        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.RemoveTask;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
        {
            switch (context.CurrentStep)
            {
                case null:
                    context.CurrentStep = "TaskId";

                    await bot.SendMessage(update.Message.Chat, "Введите Id задачи для удаления:",
                        replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.CancelContext), cancellationToken: ct);

                    return ScenarioResult.Transition;
                case "TaskId":
                    ToDoUser user = await userService.GetUser(context.UserId, ct);
                    IReadOnlyList<ToDoItem> items = await toDoService.Find(user, update.Message.Text, ct);

                    if (Guid.TryParse(update.Message.Text, out Guid taskId))
                    {
                        await toDoService.Delete(taskId, ct);

                        EnumKeyboardsScenariosTypes keyboardType;
                        if (toDoService.GetAllByUserId(user.UserId, ct).Result.Count == 0)
                            keyboardType = EnumKeyboardsScenariosTypes.NoneTasks;
                        else
                            keyboardType = EnumKeyboardsScenariosTypes.Default;

                        await bot.SetMyCommands(KeyboardCommands.KeyboardBotCommands(keyboardType), BotCommandScopeChat.Chat(update.Message.Chat.Id));
                        await bot.SendMessage(update.Message.Chat, $"Задача {taskId} удалена.",
                            replyMarkup: KeyboardCommands.CommandKeyboardMarkup(keyboardType), cancellationToken: ct);

                        return ScenarioResult.Completed;
                    }

                    await bot.SendMessage(update.Message.Chat, $"Неверно введен Id задачи, введите заново:", cancellationToken: ct);

                    return ScenarioResult.Transition;
            }
            throw new Exception("Неверный шаг сценария.");
        }
    }
}
