using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Otus_Scenario_Homework_11
{
    /// <summary>
    /// Сценарий поиска задач.
    /// </summary>
    public class FindTaskScenario : IScenario
    {
        public FindTaskScenario(IToDoService toDoService, IUserService userService)
        {
            this.toDoService = toDoService;
            this.userService = userService;
        }

        private readonly IToDoService toDoService;
        private readonly IUserService userService;
        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.FindTask;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
        {
            switch (context.CurrentStep)
            {
                case null:
                    context.UserId = update.Message.From.Id;
                    context.CurrentStep = "Prfix";

                    await bot.SetMyCommands(KeyboardCommands.KeyboardBotCommands(EnumKeyboardsScenariosTypes.CancelContext), BotCommandScopeChat.Chat(update.Message.Chat.Id));
                    await bot.SendMessage(update.Message.Chat, "Введите префикс для поиска:",
                        replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.CancelContext), cancellationToken: ct);

                    return ScenarioResult.Transition;
                case "Prfix":
                    ToDoUser user = await userService.GetUser(context.UserId, ct);
                    IReadOnlyList<ToDoItem> items = await toDoService.Find(user, update.Message.Text, ct);

                    await bot.SetMyCommands(KeyboardCommands.KeyboardBotCommands(EnumKeyboardsScenariosTypes.Default), BotCommandScopeChat.Chat(update.Message.Chat.Id));
                    await bot.SendMessage(update.Message.Chat, await ToDoListInString(items, ct), parseMode: ParseMode.Html,
                        replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Default), cancellationToken: ct);

                    return ScenarioResult.Completed;
            }
            throw new Exception("Неверный шаг сценария.");
        }

        /// <summary>
        /// Преобразование массива задач в строку для последующего вывода в консоль.
        /// </summary>
        /// <param name="toDoItems">Список задач.</param>
        /// <param name="activeStateFlag">true для вывода активных задач, false для вывода всех задач.</param>
        /// <returns>Сформированная строка.</returns>
        private Task<string> ToDoListInString(IReadOnlyList<ToDoItem> toDoItems, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            StringBuilder sbResult = new StringBuilder("");
            int itemsCount = toDoItems.Count;

            if (itemsCount == 0)
                return Task.FromResult(sbResult.Append("Задачи, удовлетворяющие условию поиску, не найдены!").ToString());

            for (int i = 0; i < itemsCount; i++)
                sbResult.Append($"{i + 1}. ({toDoItems[i].State}) {toDoItems[i].Name} - задача создана {toDoItems[i].CreatedAt}, задачу нужно выполнить до {toDoItems[i].DeadLine} - <code>{toDoItems[i].Id}</code>\r\n");

            return Task.FromResult(sbResult.ToString().Trim());
        }
    }
}
