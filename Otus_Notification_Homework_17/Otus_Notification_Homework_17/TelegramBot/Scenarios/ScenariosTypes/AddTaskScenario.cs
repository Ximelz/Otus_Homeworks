using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Otus_Notification_Homework_17
{
    /// <summary>
    /// Сценарий добавления задачи.
    /// </summary>
    public class AddTaskScenario : IScenario
    {
        public AddTaskScenario(IToDoService iToDoService, IUserService iUserService, IToDoListService iToDoListService)
        {
            this.iToDoService = iToDoService;
            this.iUserService = iUserService;
            this.iToDoListService = iToDoListService;
        }

        private readonly IToDoService iToDoService;
        private readonly IUserService iUserService;
        private readonly IToDoListService iToDoListService;
        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.AddTask;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken ct)
        {
            ToDoUser user;
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
                        context.Data.Add("DeadLine", (object)deadline);
                        context.CurrentStep = "ToDoList";

                        user = await iUserService.GetUser(context.UserId, ct);

                        IReadOnlyList<ToDoList> toDoList = await iToDoListService.GetUserLists(user.UserId, ct);

                        InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup();

                        keyboard.AddNewRow(InlineKeyboardButton.WithCallbackData("📌Без списка", ToDoListCallbackDto.FromString("addtask").ToString()));

                        if (toDoList.Count > 0)
                            foreach (ToDoList currentlist in toDoList)
                                keyboard.AddNewRow(InlineKeyboardButton.WithCallbackData(currentlist.Name, ToDoListCallbackDto.FromString($"addtask|{currentlist.Id}").ToString()));

                        await bot.SendMessage(update.Message.Chat, $"Выберите список для задачи!",
                            replyMarkup: keyboard, cancellationToken: ct);

                        return ScenarioResult.Transition;
                    }
                    await bot.SendMessage(update.Message.Chat, "Дата введена неверно! Введите дату в формате dd.MM.yyyy", cancellationToken: ct);
                    return ScenarioResult.Transition;
                case "ToDoList":
                    user = await iUserService.GetUser(context.UserId, ct);
                    ToDoListCallbackDto listDto = ToDoListCallbackDto.FromString(update.CallbackQuery.Data);
                    
                    IReadOnlyList<ToDoList> lists = await iToDoListService.GetUserLists(user.UserId, ct);
                    ToDoList? list = lists.Where(x => x.Id == listDto.ToDoListId).FirstOrDefault();
                    try
                    {
                        ToDoItem item = await iToDoService.Add(user, context.Data["Name"].ToString(), Convert.ToDateTime(context.Data["DeadLine"]), list, ct);
                        await bot.SendMessage(update.CallbackQuery.Message.Chat, $"Задача {item.Name} добавлена!",
                            replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Default), cancellationToken: ct);
                    }
                    catch (Exception ex)
                    {
                        await bot.SendMessage(update.CallbackQuery.Message.Chat, ex.Message,
                            replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Default), cancellationToken: ct);
                    }

                    return ScenarioResult.Completed;           
            }
            throw new ArgumentException("Неверный шаг сценария.");
        }
    }
}
