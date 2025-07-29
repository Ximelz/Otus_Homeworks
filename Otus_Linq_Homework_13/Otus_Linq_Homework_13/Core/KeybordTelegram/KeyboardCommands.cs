using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Otus_Linq_Homework_13
{
    public static class KeyboardCommands
    {
        /// <summary>
        /// ����� ��������� ������ ��� ���� telegram.
        /// </summary>
        /// <param name="maxTasks">true ���� ��������� ����� �� ���������� �����, false ���� �������� ������ ��� ��������.</param>
        /// <returns>������ ������ ��� ���� telegram.</returns>
        public static List<BotCommand> KeyboardBotCommands(EnumKeyboardsScenariosTypes type)
        {
            List<BotCommand> currentCommands = new List<BotCommand>();

            if (type == EnumKeyboardsScenariosTypes.CancelContext)
                currentCommands.Add(new BotCommand("/cancel", "������ ���������� ��������."));

            if (type == EnumKeyboardsScenariosTypes.Start)
                currentCommands.Add(new BotCommand("/start", "����������� � ������ ����."));
            
            if (type == EnumKeyboardsScenariosTypes.NoneTasks || type == EnumKeyboardsScenariosTypes.Default)
                currentCommands.Add(new BotCommand("/addtask", "���������� ������."));

            if (type == EnumKeyboardsScenariosTypes.Default || type == EnumKeyboardsScenariosTypes.MaxTasks)
            {
                currentCommands.Add(new BotCommand("/show", "����� ����� ������������."));
                currentCommands.Add(new BotCommand("/find", "����� �����."));
                currentCommands.Add(new BotCommand("/report", "����� ���������� ������������."));
            }
            
            currentCommands.Add(new BotCommand("/info", "���������� � ������ ����."));
            currentCommands.Add(new BotCommand("/help", "���������� � ������ � �����."));
            return currentCommands;
        }

        /// <summary>
        /// ����� ��������� ������ ��� ������ ��� ������� ��������� telegram.
        /// </summary>
        /// <returns>������ ������.</returns>
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
                    new KeyboardButton("/addtask"),
                    new KeyboardButton("/show"),
                    new KeyboardButton("/report")
                });
        }
    }
}
