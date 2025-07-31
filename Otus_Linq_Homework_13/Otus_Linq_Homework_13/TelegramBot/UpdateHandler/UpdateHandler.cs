using System.Security.Authentication;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Otus_Linq_Homework_13
{
    /// <summary>
    /// Класс обработки введенных команд.
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

        public const string helpInfo = "Для работы с ботом нужно ввести команду. В программе существуют следующие команды:\n\r" +
                                       "/info - получите информацию о версии и дате создания и изменения программы.\n\r" +
                                       "/help - получите информацию о работе с программой.\n\r" +
                                       "/start - команда для авторизации пользователя в боте. После авторизации становятся доступны следующие команды:\n\r" +
                                       "/show - команда показывает списки задач пользователя и кнопки добавления и удаления списка. " +
                                                    "При добавлении списка нужно будет следующим сообщением ввести имя этого списка, а при удалении появятся кнопки \"Да\" и \"Нет\" для подтверждения или отмены удаления." +
                                                    "При удалении списка удаляются также все задачи в этом списке." +
                                                    "При выборе определенного списка будут выведены в чат все активные задачи из этого списка в виде кнопок, а так же кнопка просмотра уже выполненных задач." +
                                                    "Вывод всех активных и, при дальнейшем переходе, выполненных задач пользователя происходит в виде списка по 5 задач и кнопками \"->\" и \"<-\", которые позволяют пролистывать список." +
                                                    "Если одной или обоих кнопок нет, то это означает что вы достигли границы списка или весь список смог поместиться на экране.\n\r" +
                                       "/addtask - команда для добавления задачи. После выбора команды предлается следующим сообщением ввести имя задачи, " +
                                                    "после этого, следующим сообщением, крайний срок выполнения задачи. Следом в виде кнопок появляются различные списки задач, в которые можно добавить эту задачу. " +
                                                    "Выбрать можно только 1 список, после чего зачада будет сохранена. \n\r" +
                                       "/report - команда для отображение статистики по задачам текущего пользователя.\n\r" +
                                       "/find - команда для поиска задачи. После выбора команды предлается следующим сообщением ввести префикс для поиска.\n\r" +
                                       "/cancel - команда завершает обработку сценария с последовательным вводом аргументов" +
                                       "Для окончания работы с ботом нужно нажать ангийскую клавишу \"A\" в консоле.";
        public const string version = "Версия программы 0.12, дата создания 20.02.2025, дата изменения 29.07.2025";

        private List<BotCommand> currentCommands;                           //Список доступных команд.
        private readonly IUserService userService;                          //Интерфейс для регистрации пользователя.
        private readonly IToDoService toDoService;                          //Интерфейс для взаимодействия с задачами.
        private readonly IToDoListService toDoListService;                  //Интерфейс для взаимодействия с задачами.
        private ReplyMarkup keyboard;                                       //Reply клавиатура телеграмм бота.
        public delegate void MessageEventHandler(string message);           //Делегат для событий.
        public event MessageEventHandler? OnHandleUpdateStarted;            //Событие начала обработки введенного сообщения.
        public event MessageEventHandler? OnHandleUpdateCompleted;          //Событие конца обработки введенного сообщения.
        private static int _pageSize = 5;                                   //Количество кнопок отображаемых задач.

        private readonly IEnumerable<IScenario> scenarios;
        private readonly IScenarioContextRepository contextRepository;

        /// <summary>
        /// Метод обработки обновления задач.
        /// </summary>
        /// <param name="botClient">Текущая сессия бота.</param>
        /// <param name="update">Объект с информацией о введенном сообщении (кто, откуда и что введено).</param>
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
        /// Метод вывода ошибки в консоль.
        /// </summary>
        /// <param name="botClient">Клиент телеграмм бота.</param>
        /// <param name="exception">Исключение.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            Console.WriteLine(exception.ToString());
            return Task.CompletedTask;
        }


        /// <summary>
        /// Метод обработки текста из чата с ботом.
        /// </summary>
        /// <param name="botClient">Чат с пользователем бота.</param>
        /// <param name="update">Объект сообщения.</param>
        /// <param name="ct">Токен отмены.</param>
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
                    await botClient.SendMessage(update.Message.Chat, "Сценарий прекратил свое выполнение.",
                        replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Default), cancellationToken: ct);
                    currentCommands = KeyboardCommands.KeyboardBotCommands(EnumKeyboardsScenariosTypes.Default);
                }
                else
                    await ProcessScenario(botClient, context, update, ct);
            }

            OnHandleUpdateCompleted?.Invoke(update.Message.Text);
        }


        /// <summary>
        /// Метод обработки inline кнопок.
        /// </summary>
        /// <param name="botClient">Чат с пользователем бота.</param>
        /// <param name="update">Объект сообщения.</param>
        /// <param name="ct">Токен отмены.</param>
        /// <returns></returns>
        private async Task OnCallbackQuery(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            ToDoUser? user = await userService.GetUser(update.CallbackQuery.From.Id, ct);

            if (user == null)
            {
                await botClient.SendMessage(update.CallbackQuery.Message.Chat, "Пользователь не авторизован!",
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

            switch (CbD.Action)
            {
                case "show":
                    await ShowList(botClient, update, user, ct);
                    break;
                case "show_completed":
                    await ShowCompletedList(botClient, update, user, ct);
                    break;
                case "showtask":
                    await ShowTask(botClient, update, user, ct);
                    break;
                case "completetask":
                    await CompleteTask(botClient, update, user, ct);
                    break;
                case "deletetask":
                    await DeleteTask(botClient, update, ct);
                    break;
                case "addlist":
                    await AddList(botClient, update, ct);
                    break;
                case "deletelist":
                    await DeleteList(botClient, update, ct);
                    break;
                default:
                    await botClient.SendMessage(update.CallbackQuery.Message.Chat, "Неизвестная кнопка!", parseMode: ParseMode.Html,
                                replyMarkup: KeyboardCommands.CommandKeyboardMarkup(EnumKeyboardsScenariosTypes.Default), cancellationToken: ct);
                    break;
            }
        }


        /// <summary>
        /// Логика обработки и ограничений по использованию команд.
        /// </summary>
        /// <param name="botClient">Текущая сессия бота</param>
        /// <param name="update">Объект с информацией о введенном сообщении (кто, откуда и что введено).</param>
        /// <param name="ct">Токен отмены</param>
        /// <returns>Результат выполнения команды.</returns>
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

            if (user == null)
                return "Пользователь не авторизовался!";

            if (update.Message.Text == "/addtask")
                return await AddTask(botClient, update, ct) + "\r\n";

            if (update.Message.Text == "/find")
                return await FindTasks(botClient, update, ct) + "\r\n";

            if (update.Message.Text == "/show")
                return await ShowTasks(user, ct) + "\r\n";

            if (update.Message.Text == "/report")
                return await ReportUserTasks(user, ct) + "\r\n";

            throw new NotSupportedException("Введена неверная команда!");
        }

        /// <summary>
        /// Проверка на авторизацию пользователя.
        /// </summary>
        /// <param name="telegramUser">Пользователь telegram.</param>
        /// <returns>Возвращает объект ConsoleUser если авторизирован, null если нет.</returns>
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
        /// Команда добавления задачи.
        /// </summary>
        /// <param name="taskName">Имя задачи.</param>
        /// <returns>Результат добавление задачи.</returns>
        private async Task<string> AddTask(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            await ProcessScenario(botClient, new ScenarioContext(ScenarioType.AddTask, update.Message.From.Id), update, ct);

            return "";
        }

        /// <summary>
        /// Авторизация и старт работы программы.
        /// </summary>
        /// <param name="telegramUser">Пользователь из telegram.</param>
        /// <returns>Результат авторизации пользователя</returns>
        private async Task<string> StartCommand(User telegramUser, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            ToDoUser user = await userService.RegisterUser(telegramUser.Id, telegramUser.Username, ct);

            keyboard = new ReplyKeyboardRemove();

            if (user == null)
                throw new AuthenticationException("Текущий пользователь не смог авторизоваться!");

            EnumKeyboardsScenariosTypes keyboardType;
            if (toDoService.GetAllByUserId(user.UserId, ct).Result.Count > 0)
                keyboardType = EnumKeyboardsScenariosTypes.Default;
            else
                keyboardType = EnumKeyboardsScenariosTypes.NoneTasks;

            currentCommands = KeyboardCommands.KeyboardBotCommands(keyboardType);
            keyboard = KeyboardCommands.CommandKeyboardMarkup(keyboardType);

            return "Пользователь авторизовался!";
        }

        /// <summary>
        /// Вывод на консоль списка задач. В зависимости от флага выводится разный список задач.
        /// </summary>
        /// <param name="user">true для вывода активных задач, false для вывода всех задач.</param>
        /// <returns>Список задач.</returns>
        private async Task<string> ShowTasks(ToDoUser user, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            IReadOnlyList<ToDoList> toDoList = await toDoListService.GetUserLists(user.UserId, ct);

            keyboard = new InlineKeyboardMarkup();

            ToDoListCallbackDto listDto = new ToDoListCallbackDto("show", null);

            ((InlineKeyboardMarkup)keyboard).AddNewRow(InlineKeyboardButton.WithCallbackData("📌Без списка", listDto.ToString()));

            if (toDoList.Count > 0)
                foreach (ToDoList list in toDoList)
                {
                    listDto = new ToDoListCallbackDto("show", list.Id);
                    ((InlineKeyboardMarkup)keyboard).AddNewRow(InlineKeyboardButton.WithCallbackData(list.Name, listDto.ToString()));
                }


            ((InlineKeyboardMarkup)keyboard).AddNewRow(new InlineKeyboardButton[] { InlineKeyboardButton.WithCallbackData("🆕Добавить", "addlist"), InlineKeyboardButton.WithCallbackData("❌Удалить", "deletelist") });

            return "Выберите список";
        }

        /// <summary>
        /// Метод вывода списка задач в чат.
        /// </summary>
        /// <param name="botClient">Текущая сессия бота</param>
        /// <param name="update">Объект с информацией о введенном сообщении (кто, откуда и что введено).</param>
        /// <param name="user">Текущий пользователь.</param>
        /// <param name="ct">Токен отмены</param>
        private async Task ShowList(ITelegramBotClient botClient, Update update, ToDoUser user, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            ToDoListCallbackDto TDListDto = ToDoListCallbackDto.FromString(update.CallbackQuery.Data);
            InlineKeyboardMarkup keyboardMarkup = new InlineKeyboardMarkup();

            var userTasksItems = await toDoService.GetByUserIdAndList(user.UserId, TDListDto.ToDoListId, ct);
            List<KeyValuePair<string, string>> tasksList = new List<KeyValuePair<string, string>>();

            foreach (var item in userTasksItems)
                tasksList.Add(new KeyValuePair<string, string>(item.Name, $"showtask|{item.Id}"));

            PagedListCallbackDto pagesDto = PagedListCallbackDto.FromString(update.CallbackQuery.Data);

            string handlerStr = tasksList.Count > 0 ? "Активные задачи" : "Задач нет!";

            ToDoItemCallbackDto TDUserTaskDto;
            foreach (var userTask in userTasksItems)
            {
                TDUserTaskDto = new ToDoItemCallbackDto("showtask", userTask.Id);
                keyboardMarkup.AddNewRow(InlineKeyboardButton.WithCallbackData(userTask.Name, TDUserTaskDto.ToString()));
            }
            keyboardMarkup = await BuildPagedButtons(tasksList, pagesDto);

            pagesDto = new PagedListCallbackDto($"show_completed", TDListDto.ToDoListId, 0);
            keyboardMarkup.AddNewRow(InlineKeyboardButton.WithCallbackData("☑️Посмотреть выполненные", pagesDto.ToString()));

            await botClient.EditMessageText(chatId: update.CallbackQuery.Message.Chat.Id, messageId: update.CallbackQuery.Message.Id, handlerStr, parseMode: ParseMode.Html,
                replyMarkup: keyboardMarkup, cancellationToken: ct);
        }


        /// <summary>
        /// Метод вывода списка выполненных задач в чат.
        /// </summary>
        /// <param name="botClient">Текущая сессия бота</param>
        /// <param name="update">Объект с информацией о введенном сообщении (кто, откуда и что введено).</param>
        /// <param name="user">Текущий пользователь.</param>
        /// <param name="ct">Токен отмены</param>
        private async Task ShowCompletedList(ITelegramBotClient botClient, Update update, ToDoUser user, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            ToDoListCallbackDto TDListDto = ToDoListCallbackDto.FromString(update.CallbackQuery.Data);
            InlineKeyboardMarkup keyboardMarkup = new InlineKeyboardMarkup();

            var userTasks = await toDoService.GetAllByUserId(user.UserId, ct);
            List<ToDoItem> userTasksCompleteItems;
            if (TDListDto == null)
                userTasksCompleteItems = userTasks.Where(x => x.State == ToDoItemState.Completed).Where(y => y.List.Id == TDListDto.ToDoListId).ToList();
            else
                userTasksCompleteItems = userTasks.Where(x => x.State == ToDoItemState.Completed).Where(y => y.List == null).ToList();

            List<KeyValuePair<string, string>> tasksList = new List<KeyValuePair<string, string>>();

            foreach (var item in userTasksCompleteItems)
                tasksList.Add(new KeyValuePair<string, string>(item.Name, $"showtask|{item.Id}"));

            PagedListCallbackDto pagesDto = PagedListCallbackDto.FromString(update.CallbackQuery.Data);
            ToDoItemCallbackDto TDUserTaskDto;

            string handlerStr = tasksList.Count > 0 ? "Выполненные задачи" : "Задач нет!";

            foreach (var userTask in userTasksCompleteItems)
            {
                TDUserTaskDto = ToDoItemCallbackDto.FromString($"showtask|{userTask.Id}");
                keyboardMarkup.AddNewRow(InlineKeyboardButton.WithCallbackData(userTask.Name, TDUserTaskDto.ToString()));
            }
            keyboardMarkup = await BuildPagedButtons(tasksList, pagesDto);

            await botClient.EditMessageText(chatId: update.CallbackQuery.Message.Chat.Id, messageId: update.CallbackQuery.Message.Id, handlerStr, parseMode: ParseMode.Html,
                replyMarkup: keyboardMarkup, cancellationToken: ct);
        }

        /// <summary>
        /// Метод вывода информации о задаче.
        /// </summary>
        /// <param name="botClient">Текущая сессия бота</param>
        /// <param name="update">Объект с информацией о введенном сообщении (кто, откуда и что введено).</param>
        /// <param name="user">Текущий пользователь.</param>
        /// <param name="ct">Токен отмены</param>
        private async Task ShowTask(ITelegramBotClient botClient, Update update, ToDoUser user, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            keyboard = new InlineKeyboardMarkup();
            ToDoItemCallbackDto TDItemDto = ToDoItemCallbackDto.FromString(update.CallbackQuery.Data);

            ToDoItemCallbackDto TDItemCompleteDto = ToDoItemCallbackDto.FromString($"completetask|{TDItemDto.ToDoItemId}");
            ToDoItemCallbackDto TDItemDeleteDto = ToDoItemCallbackDto.FromString($"deletetask|{TDItemDto.ToDoItemId}");

            ToDoItem userTask = await toDoService.Get((Guid)TDItemDto.ToDoItemId, ct);

            if (userTask.State == ToDoItemState.Active)
                ((InlineKeyboardMarkup)keyboard).AddNewRow(
                    new InlineKeyboardButton[]
                    {
                            InlineKeyboardButton.WithCallbackData("✅Выполнить", TDItemCompleteDto.ToString()),
                            InlineKeyboardButton.WithCallbackData("❌Удалить", TDItemDeleteDto.ToString())
                    });
            else
                ((InlineKeyboardMarkup)keyboard).AddNewRow(InlineKeyboardButton.WithCallbackData("❌Удалить", TDItemDeleteDto.ToString()));
            string taskInfo = $"{userTask.Name} - задача создана {userTask.CreatedAt}, задачу нужно выполнить до {userTask.DeadLine} - {userTask.Id}\r\n";
            await botClient.SendMessage(update.CallbackQuery.Message.Chat, taskInfo, parseMode: ParseMode.Html, replyMarkup: keyboard, cancellationToken: ct);
        }

        /// <summary>
        /// Метод выполнения задачи.
        /// </summary>
        /// <param name="botClient">Текущая сессия бота</param>
        /// <param name="update">Объект с информацией о введенном сообщении (кто, откуда и что введено).</param>
        /// <param name="user">Текущий пользователь.</param>
        /// <param name="ct">Токен отмены</param>
        private async Task CompleteTask(ITelegramBotClient botClient, Update update, ToDoUser user, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            ToDoItemCallbackDto TDItemDto = ToDoItemCallbackDto.FromString(update.CallbackQuery.Data);

            await toDoService.MarkCompleted((Guid)TDItemDto.ToDoItemId, user, ct);
            await botClient.EditMessageText(chatId: update.CallbackQuery.Message.Chat.Id, messageId: update.CallbackQuery.Message.Id, $"{update.CallbackQuery.Message.Text}\r\nЗадача выполнена!", parseMode: ParseMode.Html, cancellationToken: ct);
        }

        /// <summary>
        /// Метод удаления задачи.
        /// </summary>
        /// <param name="botClient">Текущая сессия бота</param>
        /// <param name="update">Объект с информацией о введенном сообщении (кто, откуда и что введено).</param>
        /// <param name="ct">Токен отмены</param>
        private async Task DeleteTask(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            await ProcessScenario(botClient, new ScenarioContext(ScenarioType.RemoveTask, update.CallbackQuery.From.Id), update, ct);
        }

        /// <summary>
        /// Метод добавления списка.
        /// </summary>
        /// <param name="botClient">Текущая сессия бота</param>
        /// <param name="update">Объект с информацией о введенном сообщении (кто, откуда и что введено).</param>
        /// <param name="ct">Токен отмены</param>
        private async Task AddList(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            await ProcessScenario(botClient, new ScenarioContext(ScenarioType.AddList, update.CallbackQuery.From.Id), update, ct);
        }

        /// <summary>
        /// Метод удаления списка.
        /// </summary>
        /// <param name="botClient">Текущая сессия бота</param>
        /// <param name="update">Объект с информацией о введенном сообщении (кто, откуда и что введено).</param>
        /// <param name="ct">Токен отмены</param>
        private async Task DeleteList(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            await ProcessScenario(botClient, new ScenarioContext(ScenarioType.DeleteList, update.CallbackQuery.From.Id), update, ct);
        }

        /// <summary>
        /// Вывод статистики пользователя по задачам.
        /// </summary>
        /// <param name="user">Пользователь, чьи задачи ищем.</param>
        /// <returns>Статистика задач пользователя.</returns>
        private async Task<string> ReportUserTasks(ToDoUser user, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            IToDoReportService reportService = new ToDoReportService(toDoService);
            var (total, completed, active, generatedAt) = await reportService.GetUserStats(user.UserId, ct);

            return $"Статистика по задачам на {generatedAt}. Всего: {total}; Завершено: {completed}; Активных: {active}";
        }

        /// <summary>
        /// Поиск задач по аргументу.
        /// </summary>
        /// <param name="inputList">Команда с аргументом.</param>
        /// <param name="user">Пользователь, чьи задачи ищем.</param>
        /// <returns>Список задач.</returns>
        private async Task<string> FindTasks(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            await ProcessScenario(botClient, new ScenarioContext(ScenarioType.FindTask, update.Message.From.Id), update, ct);

            return "";
        }

        /// <summary>
        /// Метод получения объекта сценария.
        /// </summary>
        /// <param name="scenario">Тип сценария.</param>
        /// <returns>Объект сценария.</returns>
        private IScenario GetScenario(ScenarioType scenario)
        {
            return scenarios.Where(x => x.CanHandle(scenario)).FirstOrDefault() ?? throw new ArgumentException("Сценарий не найден!");
        }

        /// <summary>
        /// Метод обработки сценариев.
        /// </summary>
        /// <param name="botClient">Сессия пользователя из Telegram.</param>
        /// <param name="context">Данные с информацией о сценарии.</param>
        /// <param name="update">Информация о сообщении из telegram.</param>
        /// <param name="ct">Токен отмены.</param>
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

        /// <summary>
        /// Метод создания кнопок с задачами на листе.
        /// </summary>
        /// <param name="callbackData"></param>
        /// <param name="listDto"></param>
        /// <returns></returns>
        private async Task<InlineKeyboardMarkup> BuildPagedButtons(IReadOnlyList<KeyValuePair<string, string>> callbackData, PagedListCallbackDto listDto)
        {
            InlineKeyboardMarkup resultKeyboard = new InlineKeyboardMarkup();
            int totalPages = callbackData.Count / _pageSize;

            var currentPageTasks = callbackData.Select(x => x.Key).ToList().GetBatchByNumber(_pageSize, listDto.Page);

            if (callbackData.Count % _pageSize != 0)
                totalPages++;

            foreach (string itemName in currentPageTasks)
                resultKeyboard.AddNewRow(InlineKeyboardButton.WithCallbackData(itemName, callbackData.Where(x => x.Key == itemName).Select(x => x.Value).First()));

            if (totalPages == 1 || totalPages == 0)
                return resultKeyboard;

            if (listDto.Page == 0)
                resultKeyboard.AddNewRow(InlineKeyboardButton.WithCallbackData("➡️", $"{listDto.Action}|{listDto.ToDoListId}|{listDto.Page + 1}"));
            else if (listDto.Page == totalPages - 1)
                resultKeyboard.AddNewRow(InlineKeyboardButton.WithCallbackData("⬅️", $"{listDto.Action}|{listDto.ToDoListId}|{listDto.Page - 1}"));
            else
                resultKeyboard.AddNewRow(new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData("⬅️", $"{listDto.Action}|{listDto.ToDoListId}|{listDto.Page - 1}"),
                        InlineKeyboardButton.WithCallbackData("➡️", $"{listDto.Action}|{listDto.ToDoListId}|{listDto.Page + 1}")
                    });

            return resultKeyboard;
        }
    }
}