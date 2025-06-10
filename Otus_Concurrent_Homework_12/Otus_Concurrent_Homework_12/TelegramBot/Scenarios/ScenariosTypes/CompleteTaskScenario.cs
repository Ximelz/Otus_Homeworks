using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Otus_Concurrent_Homework_12
{
    public class CompleteTaskScenario : IScenario
    {
        public CompleteTaskScenario(IToDoService toDoService, IUserService userService)
        {
            this.toDoService = toDoService;
            this.userService = userService;
        }

        private readonly IToDoService toDoService;
        private readonly IUserService userService;
        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.CompleteTask;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
        {
            switch (context.CurrentStep)
            {
                case null:
                    context.CurrentStep = "TaskId";

                    await bot.SendMessage(update.Message.Chat, "Введите Id задачи для выполнения:",
                        replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.CancelContext), cancellationToken: ct);

                    return ScenarioResult.Transition;
                case "TaskId":
                    ToDoUser user = await userService.GetUser(context.UserId, ct);
                    IReadOnlyList<ToDoItem> items = await toDoService.Find(user, update.Message.Text, ct);

                    if (Guid.TryParse(update.Message.Text, out Guid taskId))
                    {
                        await toDoService.MarkCompleted(taskId, user, ct);

                        await bot.SetMyCommands(KeyboardCommands.KeyboardBotCommands(EnumKeyboardsScenariosTypes.Default), BotCommandScopeChat.Chat(update.Message.Chat.Id));
                        await bot.SendMessage(update.Message.Chat, $"Задача {taskId} отмечена выполненной.",
                            replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Default), cancellationToken: ct);

                        return ScenarioResult.Completed;
                    }

                    await bot.SendMessage(update.Message.Chat, $"Неверно введен Id задачи, введите заново:", cancellationToken: ct);

                    return ScenarioResult.Transition;
            }
            throw new Exception("Неверный шаг сценария.");
        }
    }
}
