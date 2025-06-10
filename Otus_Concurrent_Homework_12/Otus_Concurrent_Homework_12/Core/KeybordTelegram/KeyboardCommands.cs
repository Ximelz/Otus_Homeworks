using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Otus_Concurrent_Homework_12
{
    public static class KeyboardCommands
    {
        /// <summary>
        /// Метод получения команд для меню telegram.
        /// </summary>
        /// <param name="maxTasks">true если достигнут лимит по количеству задач, false если добавить задачи еще возможно.</param>
        /// <returns>Список команд для меню telegram.</returns>
        public static List<BotCommand> KeyboardBotCommands(EnumKeyboardsScenariosTypes type)
        {
            List<BotCommand> currentCommands = new List<BotCommand>();

            if (type == EnumKeyboardsScenariosTypes.CancelContext)
                currentCommands.Add(new BotCommand("/cancel", "Отмена выполнения сценария."));

            if (type == EnumKeyboardsScenariosTypes.Start)
                currentCommands.Add(new BotCommand("/start", "Авторизация и запуск бота."));
            
            if (type == EnumKeyboardsScenariosTypes.NoneTasks || type == EnumKeyboardsScenariosTypes.Default)
                currentCommands.Add(new BotCommand("/addtask", "Добавление задачи."));

            if (type == EnumKeyboardsScenariosTypes.Default || type == EnumKeyboardsScenariosTypes.MaxTasks)
            {
                currentCommands.Add(new BotCommand("/showtasks", "Вывод выполненных задач пользователя."));
                currentCommands.Add(new BotCommand("/showalltasks", "Вывод всех задач пользователя."));
                currentCommands.Add(new BotCommand("/removetask", "Удаление задачи."));
                currentCommands.Add(new BotCommand("/completetask", "Выполнить задачу."));
                currentCommands.Add(new BotCommand("/find", "Поиск задач."));
                currentCommands.Add(new BotCommand("/report", "Вывод статистики пользователя."));
            }
            
            currentCommands.Add(new BotCommand("/info", "Информация о версии бота."));
            currentCommands.Add(new BotCommand("/help", "Информация о работе с ботом."));
            return currentCommands;
        }

        /// <summary>
        /// Метод получения команд для кнопок под строкой сообщения telegram.
        /// </summary>
        /// <returns>Кнопки команд.</returns>
        public static ReplyKeyboardMarkup CommandKeyboardMarkup(EnumKeyboardsScenariosTypes type)
        {
            if (type == EnumKeyboardsScenariosTypes.CancelContext)
                return new ReplyKeyboardMarkup(new[] { new KeyboardButton("/cancel") });

            if (type == EnumKeyboardsScenariosTypes.Start)
                return new ReplyKeyboardMarkup(new[] { new KeyboardButton("/start") });

            if (type == EnumKeyboardsScenariosTypes.NoneTasks)
                return new ReplyKeyboardMarkup(new[] { new KeyboardButton("/addtask") });

            return new ReplyKeyboardMarkup(
                new[]
                {
                    new KeyboardButton("/showtasks"),
                    new KeyboardButton("/showalltasks"),
                    new KeyboardButton("/report")
                });
        }
    }
}
