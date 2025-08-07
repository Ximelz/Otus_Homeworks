using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Otus_BackgroundTask_Homework_16
{
    public class DeleteListScenario : IScenario
    {
        public DeleteListScenario(IUserService userService, IToDoListService toDoListService, IToDoService toDoService)
        {
            iUserService = userService;
            iToDoListService = toDoListService;
            iToDoService = toDoService;
        }

        private readonly IToDoService iToDoService;
        private readonly IUserService iUserService;
        private readonly IToDoListService iToDoListService;

        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.DeleteList;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
        {
            ToDoUser? user = await iUserService.GetUser(context.UserId, ct);
            Chat chat = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.Chat : update.Message.Chat;
            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup();

            if (user == null)
            {
                await bot.SendMessage(chat, $"Пользователь не авторизован!",
                    replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Default), cancellationToken: ct);
                return ScenarioResult.Completed;
            }

            switch (context.CurrentStep)
            {
                case null:
                    context.CurrentStep = "Approve";

                    IReadOnlyList<ToDoList> lists = await iToDoListService.GetUserLists(user.UserId, ct);

                    foreach (var list in lists)
                        keyboard.AddNewRow(InlineKeyboardButton.WithCallbackData(list.Name, ToDoListCallbackDto.FromString($"deletelist|{list.Id}").ToString()));

                    await bot.SendMessage(chat, "Выберете список для удаления:", replyMarkup: keyboard, cancellationToken: ct);

                    return ScenarioResult.Transition;
                case "Approve":
                    context.CurrentStep = "Delete";

                    ToDoListCallbackDto listDto = ToDoListCallbackDto.FromString(update.CallbackQuery.Data);
                    ToDoList listToDel;

                    if (listDto.ToDoListId == null)
                    {
                        await bot.SendMessage(chat, "Выбран неверный список!", cancellationToken: ct);
                        return ScenarioResult.Transition;
                    }

                    listToDel = await iToDoListService.Get((Guid)listDto.ToDoListId, ct);
                    context.Data.Add("ToDoList", listToDel);

                    keyboard.AddNewRow(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("✅Да", "yes"), InlineKeyboardButton.WithCallbackData("❌Нет", "no") });

                    await bot.SendMessage(chat, $"Подтверждаете удаление списка {listToDel.Name} и всех его задач", replyMarkup: keyboard, cancellationToken: ct);

                    return ScenarioResult.Transition;
                case "Delete":

                    ToDoListCallbackDto delListDto = ToDoListCallbackDto.FromString(update.CallbackQuery.Data);
                    ToDoList removeList = (ToDoList)context.Data["ToDoList"];

                    if (delListDto.Action == "yes")
                    {
                        IReadOnlyList<ToDoItem> tasks = await iToDoService.GetByUserIdAndList(user.UserId, removeList.Id, ct);
                        foreach (ToDoItem task in tasks)
                            iToDoService.Delete(task.Id, ct);

                        await iToDoListService.Delete(removeList.Id, ct);

                        await bot.SendMessage(update.CallbackQuery.Message.Chat, $"Список {removeList.Name} удален!",
                            replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Default), cancellationToken: ct);
                    }
                    else
                        await bot.SendMessage(update.CallbackQuery.Message.Chat, $"Удаление отменено!",
                            replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Default), cancellationToken: ct);


                    return ScenarioResult.Completed;
            }
            throw new ArgumentException("Неверный шаг сценария.");
        }
    }
}
