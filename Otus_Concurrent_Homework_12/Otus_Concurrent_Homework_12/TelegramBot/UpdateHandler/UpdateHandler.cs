using System;
using System.Security.Authentication;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Otus_Concurrent_Homework_12
{
    /// <summary>
    /// ����� ��������� ��������� ������.
    /// </summary>
    public class UpdateHandler : IUpdateHandler
    {
        public UpdateHandler(IUserService userService, IToDoService iToDoService, IEnumerable<IScenario> scenarios, IScenarioContextRepository contextRepository, IToDoListService toDoListService)
        {
            this.userService = userService;
            this.toDoService = iToDoService;
            this.toDoListService = toDoListService;
            this.scenarios = scenarios;
            this.contextRepository = contextRepository;
            currentCommands = KeyboardCommands.KeyboardBotCommands(EnumKeyboardsScenariosTypes.Start);
            keyboard = KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Start);
        }

        public const string helpInfo = "��� ������ � ����� ����� ������ �������. � ��������� ���������� ��������� �������:\n\r" +
                                       "/info - �������� ���������� � ������ � ���� �������� � ��������� ���������.\n\r" +
                                       "/help - �������� ���������� � ������ � ����������.\n\r" +
                                       "/start - ������� ��� ����������� ������������ � ����. ����� ����������� ���������� �������� ��������� �������:\n\r" +
                                       "/show - ������� ���������� ������ ����� ������������ � ������ ���������� � �������� ������. " +
                                                    "��� ���������� ������ ����� ����� ��������� ���������� ������ ��� ����� ������, � ��� �������� �������� ������ \"��\" � \"���\" ��� ������������� ��� ������ ��������." +
                                                    "��� �������� ������ ��������� ����� ��� ������ � ���� ������." +
                                                    "��� ������ ������������� ������ ����� �������� � ��� ��� �������� ������ �� ����� ������.\n\r" +
                                       "/addtask - ������� ��� ���������� ������. ����� ������ ������� ���������� ��������� ���������� ������ ��� ������, " +
                                                    "����� �����, ��������� ����������, ������� ���� ���������� ������. ������ � ���� ������ ���������� ��������� ������ �����, � ������� ����� �������� ��� ������. " +
                                                    "������� ����� ������ 1 ������, ����� ���� ������ ����� ���������. \n\r" +
                                       "/removetask - �������, �������� �������� ��������� ������ �����������. ����� ������ ������� ���������� ��������� ���������� ������ id ������ ��� ��������." +
                                       "/completetask - �������, �������� �������� ��������� ������ �����������. ����� ������ ������� ���������� ��������� ���������� ������ id ������ ��� ����������." +
                                       "/report - ������� ��� ����������� ���������� �� ������� �������� ������������.\n\r" +
                                       "/find - ������� ��� ������ ������. ����� ������ ������� ���������� ��������� ���������� ������ ������� ��� ������.\n\r" +
                                       "/cancel - ������� ��������� ��������� �������� � ���������������� ������ ����������" +
                                       "��� ��������� ������ � ����� ����� ������ ��������� ������� \"A\" � �������.";
        public const string version = "������ ��������� 0.11, ���� �������� 20.02.2025, ���� ��������� 23.06.2025";

        private List<BotCommand> currentCommands;                           //������ ��������� ������.
        private readonly IUserService userService;                          //��������� ��� ����������� ������������.
        private readonly IToDoService toDoService;                          //��������� ��� �������������� � ��������.
        private readonly IToDoListService toDoListService;                          //��������� ��� �������������� � ��������.
        private ReplyMarkup keyboard;                                       //Reply ���������� ��������� ����.
        public delegate void MessageEventHandler(string message);           //������� ��� �������.
        public event MessageEventHandler? OnHandleUpdateStarted;            //������� ������ ��������� ���������� ���������.
        public event MessageEventHandler? OnHandleUpdateCompleted;          //������� ����� ��������� ���������� ���������.

        private readonly IEnumerable<IScenario> scenarios;
        private readonly IScenarioContextRepository contextRepository;

        /// <summary>
        /// ����� ��������� ���������� �����.
        /// </summary>
        /// <param name="botClient">������� ������ ����.</param>
        /// <param name="update">������ � ����������� � ��������� ��������� (���, ������ � ��� �������).</param>
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            Chat chat = update.Type == UpdateType.CallbackQuery ? update.CallbackQuery.Message.Chat : update.Message.Chat;

            try
            {
                switch (update.Type)
                {
                    case UpdateType.CallbackQuery:
                        await OnCallbackQuery(botClient, update, ct);
                        break;
                    default:
                        await OnMessage(botClient, update, ct);
                        break;
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                await botClient.SendMessage(chat, ex.Message, cancellationToken: ct);
            }
            catch (NotSupportedException ex)
            {
                await botClient.SendMessage(chat, ex.Message, cancellationToken: ct);
            }
            catch (ArgumentException ex)
            {
                await botClient.SendMessage(chat, ex.Message, cancellationToken: ct);
            }
            catch (DuplicateTaskException ex)
            {
                currentCommands = KeyboardCommands.KeyboardBotCommands(EnumKeyboardsScenariosTypes.Default);
                keyboard = KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Default);
                await botClient.SendMessage(chat, ex.Message, replyMarkup: keyboard,
                    cancellationToken: ct);
            }
            catch (TaskCountLimitException ex)
            {
                currentCommands = KeyboardCommands.KeyboardBotCommands(EnumKeyboardsScenariosTypes.MaxTasks);
                keyboard = KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.MaxTasks);
                await botClient.SendMessage(chat, ex.Message, replyMarkup: keyboard,
                    cancellationToken: ct);
            }
            catch (TaskLengthLimitException ex)
            {
                currentCommands = KeyboardCommands.KeyboardBotCommands(EnumKeyboardsScenariosTypes.Default);
                keyboard = KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Default);
                await botClient.SendMessage(chat, ex.Message, replyMarkup: keyboard,
                    cancellationToken: ct);
            }
            catch (CompleteTaskException ex)
            {
                currentCommands = KeyboardCommands.KeyboardBotCommands(EnumKeyboardsScenariosTypes.Default);
                keyboard = KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Default);
                await botClient.SendMessage(chat, ex.Message, replyMarkup: keyboard,
                    cancellationToken: ct);
            }
            catch (AuthenticationException ex)
            {
                currentCommands = KeyboardCommands.KeyboardBotCommands(EnumKeyboardsScenariosTypes.Start);
                keyboard = KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Start);
                await botClient.SendMessage(chat, ex.Message, replyMarkup: keyboard,
                    cancellationToken: ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await botClient.SetMyCommands(currentCommands, BotCommandScopeChat.Chat(chat.Id));
            }
        }

        /// <summary>
        /// ����� ������ ������ � �������.
        /// </summary>
        /// <param name="botClient">������ ��������� ����.</param>
        /// <param name="exception">����������.</param>
        /// <param name="ct">������ ������ ������.</param>
        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            Console.WriteLine(exception.ToString());
            return Task.CompletedTask;
        }


        private async Task OnMessage(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            OnHandleUpdateStarted?.Invoke(update.Message.Text);

            ScenarioContext? context = await contextRepository.GetContext(update.Message.From.Id, ct);
            if (context == null)
            {
                string returnMessage = await HandleCommand(update, botClient, ct);
                if (returnMessage.Trim() != "")
                    await botClient.SendMessage(update.Message.Chat, returnMessage, parseMode: ParseMode.Html, replyMarkup: keyboard, cancellationToken: ct);
            }
            else
            {
                if (update.Message.Text == "/cancel")
                {
                    await contextRepository.ResetContext(update.Message.From.Id, ct);
                    await botClient.SendMessage(update.Message.Chat, "�������� ��������� ���� ����������.",
                        replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Default), cancellationToken: ct);
                    currentCommands = KeyboardCommands.KeyboardBotCommands(EnumKeyboardsScenariosTypes.Default);
                }
                else
                    await ProcessScenario(botClient, context, update, ct);
            }

            OnHandleUpdateCompleted?.Invoke(update.Message.Text);
        }

        private async Task OnCallbackQuery(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            ToDoUser? user = await userService.GetUser(update.CallbackQuery.From.Id, ct);

            if (user == null)
            {
                await botClient.SendMessage(update.CallbackQuery.Message.Chat, "������������ �� �����������!",
                    replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Default), cancellationToken: ct);
                return;
            }

            ScenarioContext? context = await contextRepository.GetContext(user.TelegramUserId, ct);

            if (context != null)
            {
                await ProcessScenario(botClient, context, update, ct);
                return;
            }

            CallbackDto CbD = CallbackDto.FromString(update.CallbackQuery.Data);

            if (CbD.Action == "show")
            {
                ToDoListCallbackDto TDListDto = ToDoListCallbackDto.FromString(update.CallbackQuery.Data);
                string userTasks = await ToDoListInString(await toDoService.GetByUserIdAndList (user.UserId, TDListDto.ToDoListId, ct), ct);
                await botClient.SendMessage(update.CallbackQuery.Message.Chat, userTasks, parseMode: ParseMode.Html,
                    replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Default), cancellationToken: ct);
                return;
            }

            if (CbD.Action == "addlist")
            {
                await ProcessScenario(botClient, new ScenarioContext(ScenarioType.AddList, update.CallbackQuery.From.Id), update, ct);
                return;
            }

            if (CbD.Action == "deletelist")
            {
                await ProcessScenario(botClient, new ScenarioContext(ScenarioType.DeleteList, update.CallbackQuery.From.Id), update, ct);
                return;
            }

            await botClient.SendMessage(update.CallbackQuery.Message.Chat, "����������� ������!", parseMode: ParseMode.Html,
                        replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Default), cancellationToken: ct);
        }
        

        /// <summary>
        /// ������ ��������� � ����������� �� ������������� ������.
        /// </summary>
        /// <param name="inputList">������� ������, ��������� �� ������� � ����������.</param>
        /// <param name="telegramUsere">������������ �� telegram.</param>
        /// <returns>��������� ���������� �������.</returns>
        private async Task<string> HandleCommand(Update update, ITelegramBotClient botClient, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            if (update.Message.Text == "/start")
                return await StartCommand(update.Message.From, ct) + "\r\n";

            if (update.Message.Text == "/help")
                return helpInfo + "\r\n";

            if (update.Message.Text == "/info")
                return version + "\r\n";

            ToDoUser? user = await CheckAuthUser(update.Message.From, ct);

            if (update.Message.Text == "/addtask")
                return await AddTask(botClient, update, ct) + "\r\n";

            if (update.Message.Text == "/find")
                return await FindTasks(botClient, update, ct) + "\r\n";

            if (update.Message.Text == "/show")
                return await ShowTasks(user, ct) + "\r\n";

            if (update.Message.Text == "/removetask")
                return await RemoveTask(botClient, update, ct) + "\r\n";

            if (update.Message.Text == "/completetask")
                return await CompleteTask(botClient, update, ct) + "\r\n";

            if (update.Message.Text == "/report")
                return await ReportUserTasks(user, ct) + "\r\n";

            throw new NotSupportedException("������� �������� �������!");
        }

        /// <summary>
        /// �������� �� ����������� ������������.
        /// </summary>
        /// <param name="telegramUser">������������ telegram.</param>
        /// <returns>���������� ������ ConsoleUser ���� �������������, null ���� ���.</returns>
        private async Task<ToDoUser?> CheckAuthUser(User telegramUser, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            ToDoUser? user = await userService.GetUser(telegramUser.Id, ct);
            if (user != null)
                return user;

            return null;
        }

        /// <summary>
        /// ������� ���������� ������.
        /// </summary>
        /// <param name="taskName">��� ������.</param>
        /// <returns>��������� ���������� ������.</returns>
        private async Task<string> AddTask(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            await ProcessScenario(botClient, new ScenarioContext(ScenarioType.AddTask, update.Message.From.Id), update, ct);

            return "";
        }

        /// <summary>
        /// ����������� � ����� ������ ���������.
        /// </summary>
        /// <param name="telegramUser">������������ �� telegram.</param>
        /// <returns>��������� ����������� ������������</returns>
        private async Task<string> StartCommand(User telegramUser, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            ToDoUser user = await userService.RegisterUser(telegramUser.Id, telegramUser.Username, ct);

            keyboard = new ReplyKeyboardRemove();

            if (user == null)
                throw new AuthenticationException("������� ������������ �� ���� ��������������!");

            EnumKeyboardsScenariosTypes keyboardType;
            if (toDoService.GetAllByUserId(user.UserId, ct).Result.Count > 0)
                keyboardType = EnumKeyboardsScenariosTypes.Default;
            else
                keyboardType = EnumKeyboardsScenariosTypes.NoneTasks;

            currentCommands = KeyboardCommands.KeyboardBotCommands(keyboardType);
            keyboard = KeyboardCommands.CommandKeyboardMarkup(keyboardType);

            return "������������ �������������!";
        }

        /// <summary>
        /// ����� �� ������� ������ �����. � ����������� �� ����� ��������� ������ ������ �����.
        /// </summary>
        /// <param name="user">true ��� ������ �������� �����, false ��� ������ ���� �����.</param>
        /// <returns>������ �����.</returns>
        private async Task<string> ShowTasks(ToDoUser user, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            IReadOnlyList<ToDoList> toDoList = await toDoListService.GetUserLists(user.UserId, ct);

            keyboard = new InlineKeyboardMarkup();

            ToDoListCallbackDto listDto = new ToDoListCallbackDto("show", null);

            ((InlineKeyboardMarkup)keyboard).AddNewRow(InlineKeyboardButton.WithCallbackData("��� ������", listDto.ToString()));

            if (toDoList.Count > 0)
                foreach (ToDoList list in toDoList)
                {
                    listDto = new ToDoListCallbackDto("show", list.Id);
                    ((InlineKeyboardMarkup)keyboard).AddNewRow(InlineKeyboardButton.WithCallbackData(list.Name, listDto.ToString()));
                }


            ((InlineKeyboardMarkup)keyboard).AddNewRow(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("��������", "addlist"), InlineKeyboardButton.WithCallbackData("�������", "deletelist") });

            return "�������� ������";
        }

        /// <summary>
        /// �������� ������.
        /// </summary>
        /// <param name="inputList">������ � �������� � �����������.</param>
        /// <returns>��������� �������� ������.</returns>
        private async Task<string> RemoveTask(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            await ProcessScenario(botClient, new ScenarioContext(ScenarioType.RemoveTask, update.Message.From.Id), update, ct);

            return "";
        }

        /// <summary>
        /// ������� � ���������� ������.
        /// </summary>
        /// <param name="inputList">������ � �������� � �����������.</param>
        /// <returns>��������� ������� ���������� ������.</returns>
        private async Task<string> CompleteTask(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            await ProcessScenario(botClient, new ScenarioContext(ScenarioType.CompleteTask, update.Message.From.Id), update, ct);

            return "";
        }

        /// <summary>
        /// ����� ���������� ������������ �� �������.
        /// </summary>
        /// <param name="user">������������, ��� ������ ����.</param>
        /// <returns>���������� ����� ������������.</returns>
        private async Task<string> ReportUserTasks(ToDoUser user, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            IToDoReportService reportService = new ToDoReportService(toDoService);
            var (total, completed, active, generatedAt) = await reportService.GetUserStats(user.UserId, ct);

            return $"���������� �� ������� �� {generatedAt}. �����: {total}; ���������: {completed}; ��������: {active}";
        }

        /// <summary>
        /// ����� ����� �� ���������.
        /// </summary>
        /// <param name="inputList">������� � ����������.</param>
        /// <param name="user">������������, ��� ������ ����.</param>
        /// <returns>������ �����.</returns>
        private async Task<string> FindTasks(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            await ProcessScenario(botClient, new ScenarioContext(ScenarioType.FindTask, update.Message.From.Id), update, ct);

            return "";
        }

        /// <summary>
        /// �������������� ������� ����� � ������ ��� ������������ ������ � �������.
        /// </summary>
        /// <param name="toDoItems">������ �����.</param>
        /// <returns>�������������� ������.</returns>
        private Task<string> ToDoListInString(IReadOnlyList<ToDoItem> toDoItems, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            StringBuilder sbResult = new StringBuilder("");
            string tempString;
            int itemsCount = toDoItems.Count;

            if (itemsCount == 0)
                return Task.FromResult(sbResult.Append("������, ��������������� ������� ������, �� �������!").ToString());

            for (int i = 0; i < itemsCount; i++)
            {
                sbResult.Append($"{i + 1}. {toDoItems[i].Name} - ������ ������� {toDoItems[i].CreatedAt}, ������ ����� ��������� �� {toDoItems[i].DeadLine} - <code>{toDoItems[i].Id}</code>\r\n");
            }

            return Task.FromResult(sbResult.ToString().Trim());
        }

        /// <summary>
        /// ����� ��������� ������� ��������.
        /// </summary>
        /// <param name="scenario">��� ��������.</param>
        /// <returns>������ ��������.</returns>
        private IScenario GetScenario(ScenarioType scenario)
        {
            return scenarios.Where(x => x.CanHandle(scenario)).FirstOrDefault() ?? throw new ArgumentException("�������� �� ������!");
        }

        /// <summary>
        /// ����� ��������� ���������.
        /// </summary>
        /// <param name="botClient">������ ������������ �� Telegram.</param>
        /// <param name="context">������ � ����������� � ��������.</param>
        /// <param name="update">���������� � ��������� �� telegram.</param>
        /// <param name="ct">����� ������.</param>
        private async Task ProcessScenario(ITelegramBotClient botClient, ScenarioContext context, Update update, CancellationToken ct)
        {
            IScenario scenario = GetScenario(context.CurrentScenario);
            ScenarioResult scenarioResult = await scenario.HandleMessageAsync(botClient, context, update, ct);

            long userId;

            if (update.Type == UpdateType.CallbackQuery)
                userId = update.CallbackQuery.From.Id;
            else
                userId = update.Message.From.Id;

            if (scenarioResult == ScenarioResult.Completed)
                contextRepository.ResetContext(userId, ct);
            else if (scenarioResult == ScenarioResult.Transition)
                contextRepository.SetContext(userId, context, ct);
        }

    }
}