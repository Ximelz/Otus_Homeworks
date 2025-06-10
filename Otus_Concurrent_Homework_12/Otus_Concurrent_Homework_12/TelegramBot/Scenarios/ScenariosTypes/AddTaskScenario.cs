using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Otus_Concurrent_Homework_12
{
    /// <summary>
    /// Сценарий добавления задачи.
    /// </summary>
    public class AddTaskScenario : IScenario
    {
        public AddTaskScenario(IToDoService iToDoService, IUserService iUserService)
        {
            this.iToDoService = iToDoService;
            this.iUserService = iUserService;
        }

        private readonly IToDoService iToDoService;
        private readonly IUserService iUserService;
        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.AddTask;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
        {
            switch (context.CurrentStep)
            {
                case null:
                    context.CurrentStep = "Name";

                    await bot.SendMessage(update.Message.Chat, "Введите название задачи:",
                        replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.CancelContext), cancellationToken: ct);

                    return ScenarioResult.Transition;
                case "Name":
                    context.Data.Add("Name", update.Message.Text);
                    context.CurrentStep = "Deadline";

                    await bot.SendMessage(update.Message.Chat, "Введите крайнюю дату выполнения задачи:",
                        replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.CancelContext), cancellationToken: ct);

                    return ScenarioResult.Transition;
                case "Deadline":
                    string format =  "dd.MM.yyyy" ;
                    if (DateTime.TryParseExact(update.Message.Text, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime deadline))
                    {
                        ToDoUser user = await iUserService.GetUser(context.UserId, ct);
                        ToDoItem item = await iToDoService.Add(user, context.Data["Name"].ToString(), deadline, ct);

                        await bot.SendMessage(update.Message.Chat, $"Задача {item.Name} добавлена!", 
                            replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Default), cancellationToken: ct);

                        return ScenarioResult.Completed;
                    }
                    await bot.SendMessage(update.Message.Chat, "Дата введена неверно! Введите дату в формате dd.MM.yyyy", cancellationToken: ct);
                    return ScenarioResult.Transition;
            }
            throw new ArgumentException("Неверный шаг сценария.");
        }
    }
}
