using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Otus_Notification_Homework_17
{
    public class AddListScenario : IScenario
    {
        public AddListScenario(IUserService userService, IToDoListService toDoListService)
        {
            iUserService = userService;
            iToDoListService = toDoListService;
        }

        private readonly IUserService iUserService;
        private readonly IToDoListService iToDoListService;

        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.AddList;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
        {
            ToDoUser? user = await iUserService.GetUser(context.UserId, ct);
            Chat chat = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.Chat : update.Message.Chat;

            if (user == null)
            {
                await bot.SendMessage(chat, $"Пользователь не авторизован!",
                    replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Default), cancellationToken: ct);
                return ScenarioResult.Completed;
            }

            switch (context.CurrentStep)
            {
                case null:
                    context.CurrentStep = "Name";

                    await bot.SendMessage(chat, "Введите название списка задач:",
                        replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.CancelContext), cancellationToken: ct);

                    return ScenarioResult.Transition;
                case "Name":
                    iToDoListService.Add(user, update.Message.Text, ct);

                    await bot.SendMessage(chat, $"Список {update.Message.Text} добавлен!",
                        replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Default), cancellationToken: ct);

                    return ScenarioResult.Completed;
            }
            throw new ArgumentException("Неверный шаг сценария.");
        }
    }
}
