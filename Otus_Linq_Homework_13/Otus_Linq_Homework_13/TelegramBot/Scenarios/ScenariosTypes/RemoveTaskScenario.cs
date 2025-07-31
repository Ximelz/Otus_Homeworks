using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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
            if (UpdateType.CallbackQuery != update.Type)
            {
                await bot.SendMessage(update.Message.Chat, $"Необходимо нажать на кнопку!", cancellationToken: ct);
                return ScenarioResult.Transition;
            }

            ToDoUser? user = await userService.GetUser(context.UserId, ct);
            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup();
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
                    context.CurrentStep = "Delete";

                    ToDoItem item = await toDoService.Get((Guid)ToDoItemCallbackDto.FromString(update.CallbackQuery.Data).ToDoItemId, ct);

                    if (item == null)
                    {
                        await bot.SendMessage(chat, $"Задача не найдена!?", cancellationToken: ct);
                        return ScenarioResult.Completed;
                    }

                    context.Data.Add("ToDoItemId", item.Id);

                    keyboard.AddNewRow(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("✅Да", "yes"), InlineKeyboardButton.WithCallbackData("❌Нет", "no") });

                    await bot.SendMessage(chat, $"Подтверждаете удаление задачи {item.Name}?", replyMarkup: keyboard, cancellationToken: ct);
                    return ScenarioResult.Transition;
                case "Delete":
                    if (update.CallbackQuery.Data == "yes")
                    {
                        if (Guid.TryParse(context.Data["ToDoItemId"].ToString(), out Guid taskId))
                        {
                            await toDoService.Delete(taskId, ct);

                            EnumKeyboardsScenariosTypes keyboardType;
                            if (toDoService.GetAllByUserId(user.UserId, ct).Result.Count == 0)
                                keyboardType = EnumKeyboardsScenariosTypes.NoneTasks;
                            else
                                keyboardType = EnumKeyboardsScenariosTypes.Default;

                            await bot.SetMyCommands(KeyboardCommands.KeyboardBotCommands(keyboardType), BotCommandScopeChat.Chat(chat.Id));
                            await bot.SendMessage(chat, $"Задача {taskId} удалена.",
                                replyMarkup: KeyboardCommands.CommandKeyboardMarkup(keyboardType), cancellationToken: ct);

                            return ScenarioResult.Completed;
                        }
                    }
                    else if (update.CallbackQuery.Data == "no")
                    {
                        await bot.SendMessage(chat, $"Удаление отменено!", cancellationToken: ct);
                        return ScenarioResult.Completed;
                    }
                    await bot.SendMessage(chat, $"Нажата неверная кнопка!", cancellationToken: ct);
                    return ScenarioResult.Transition;
            }
            throw new Exception("Неверный шаг сценария.");
        }
    }
}
