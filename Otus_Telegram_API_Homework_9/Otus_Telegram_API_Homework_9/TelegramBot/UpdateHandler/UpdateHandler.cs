using Otus_Telegram_API_Homework_9.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Otus_Telegram_API_Homework_9
{
    /// <summary>
    /// Класс обработки введенных команд.
    /// </summary>
    public class UpdateHandler : IUpdateHandler
    {
        public UpdateHandler(IUserService userService, IToDoService iToDoService)
        {
            this.userService = userService;
            this.toDoService = iToDoService;
            currentCommands = new List<BotCommand>
            {
                new BotCommand("/start", "Авторизация и запуск бота."),
                new BotCommand("/info", "Информация о версии бота."),
                new BotCommand("/help", "Информация о работе с ботом.")
            };
            keyboard = new ReplyKeyboardMarkup(new[] { new KeyboardButton("/start")});
        }

        public const string helpInfo = "Для работы с ботом нужно ввести команду. В программе существуют следующие команды:\n\r" +
                                       "/info - получите информацию о версии и дате создания и изменения программы.\n\r" +
                                       "/help - получите информацию о работе с программой.\n\r" +
                                       "/start - команда для авторизации пользователя в боте. После авторизации становятся доступны следующие команды:\n\r" +
                                       "/showtasks - команда показывает все активные задачи.\n\r" +
                                       "/showalltasks - команда показывает все имеющиеся задачи.\n\r" +
                                       "/addtask {name} - команда для добавления задачи. В качестве аргумента {name} передается имя задачи.\n\r" +
                                       "/removetask {id} - команда, котороая отмечает выбранную задачу выполненной. В качестве аргумента {id} передается id задачи." +
                                       "/completetask {id} - команда, котороая отмечает выбранную задачу выполненной. В качестве аргумента {id} передается id задачи." +
                                       "/report - команда для отображение статистики по задачам текущего пользователя.\n\r" +
                                       "/find {str} - команда для поиска задачи с префиксом {str} текущего пользователя.\n\r" +
                                       "Для окончания работы с ботом нужно нажать ангийскую клавишу \"A\" в консоле.";
        public const string version = "Версия программы 0.8, дата создания 20.02.2025, дата изменения 10.05.2025";

        private readonly List<BotCommand> currentCommands;                  //Список доступных команд.
        private readonly IUserService userService;                          //Интерфейс для регистрации пользователя.
        private readonly IToDoService toDoService;                          //Интерфейс для взаимодействия с задачами.
        private ReplyMarkup keyboard;                                       //Reply клавиатура телеграмм бота.
        public delegate void MessageEventHandler(string message);           //Делегат для событий.
        public event MessageEventHandler? OnHandleUpdateStarted;            //Событие начала обработки введенного сообщения.
        public event MessageEventHandler? OnHandleUpdateCompleted;          //Событие конца обработки введенного сообщения.


        /// <summary>
        /// Метод обработки обновления задач.
        /// </summary>
        /// <param name="botClient">Текущая сессия бота.</param>
        /// <param name="update">Объект с информацией о введенном сообщении (кто, откуда и что введено).</param>
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            try
            {
                OnHandleUpdateStarted?.Invoke(update.Message.Text);

                List<string> messageInList = await StringArrayHandler(botClient, update, ct);
                await botClient.SendMessage(update.Message.Chat, await HandleCommand(messageInList, update.Message.From, ct), parseMode: ParseMode.Html, replyMarkup: keyboard, cancellationToken: ct);

                OnHandleUpdateCompleted?.Invoke(update.Message.Text);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: ct);
            }
            catch (NotSupportedException ex)
            {
                await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: ct);
            }
            catch (ArgumentException ex)
            {
                await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: ct);
            }
            catch (DuplicateTaskException ex)
            {
                await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: ct);
            }
            catch (TaskCountLimitException ex)
            {
                await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: ct);
            }
            catch (TaskLengthLimitException ex)
            {
                await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: ct);
            }
            catch (CompleteTaskException ex)
            {
                await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: ct);
            }
            catch (AuthenticationException ex)
            {
                keyboard = new ReplyKeyboardMarkup(new[] { new KeyboardButton("/start") });
                currentCommands.Clear();
                currentCommands.Add(new BotCommand("/start", "Авторизация и запуск бота."));
                currentCommands.Add(new BotCommand("/info", "Информация о версии бота."));
                currentCommands.Add(new BotCommand("/help", "Информация о работе с ботом."));
                await botClient.SendMessage(update.Message.Chat, ex.Message, replyMarkup: keyboard, cancellationToken: ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await botClient.SetMyCommands(currentCommands, BotCommandScopeChat.Chat(update.Message.Chat.Id));
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
        /// Обработка аргументов введенной строки.
        /// </summary>
        /// <param name="message">Введенная строка.</param>
        /// <returns>Обработанный массив команды с аргументами.</returns>
        private async Task<List<string>> StringArrayHandler(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            string message = update.Message.Text;
            
            List<string> inputList = await DeleteNullItemsArray(new List<string>(message.Split(' ')), ct);

            var commands = await botClient.GetMyCommands(BotCommandScopeChat.Chat(update.Message.Chat.Id));

            if (commands.Where(x => "/" + x.Command == inputList[0]).FirstOrDefault() == null)
                throw new NotSupportedException("Введена неверная команда!");

            if (inputList[0] == "/addtask" || inputList[0] == "/find")
                if (inputList.Count > 1)
                    return await ConcatArgsInArray(inputList, ct);
                else
                    throw new ArgumentException($"Введено неверное количество аргументов для команды {inputList[0]}.");

            if (inputList[0] == "/removetask" || inputList[0] == "/completetask")
            {
                if (inputList.Count != 2)
                    throw new ArgumentException($"Введено неверное количество аргументов для команды {inputList[0]}. Их должно быть 3!");
                return inputList;
            }

            if (inputList.Count == 1)
                return inputList;
                        
            throw new ArgumentException($"Введено неверное количество аргументов для команды {inputList[0]}.");
        }

        /// <summary>
        /// Удаление пустых элементов списка.
        /// </summary>
        /// <param name="inputList">Входной список.</param>
        /// <returns>Отформатированный список.</returns>
        private async Task<List<string>> DeleteNullItemsArray(List<string> inputList, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            List<string> tempList = new List<string>();

            foreach (string item in inputList)
                if (await ValidateString(item, ct))
                    tempList.Add(item);

            return tempList;
        }

        /// <summary>
        /// Проверка строки на пустоту или null.
        /// </summary>
        /// <param name="item">Входная строка.</param>
        /// <returns>true если строка не пустая, false если строка null, пустая или состоит из пробелов.</returns>
        private Task<bool> ValidateString(string item, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            if (item == null)
                return Task.FromResult(false);

            if (item.Trim() == "")
                return Task.FromResult(false);

            return Task.FromResult(true);
        }

        /// <summary>
        /// Объединение всех элементов списка начиная со 2 элемента.
        /// </summary>
        /// <param name="inputList">Входной список.</param>
        /// <returns>Объединенный список из 2-х элементов.</returns>
        private Task<List<string>> ConcatArgsInArray(List<string> inputList, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            List<string> tempArray = new List<string>();
            int inputArrayLength = inputList.Count;
            var tempsb = new StringBuilder("");

            tempArray.Add(inputList[0]);

            for (int i = 1; i < inputArrayLength; i++)
                tempsb.Append(inputList[i] + " ");

            tempArray.Add(tempsb.ToString().Trim());

            return Task.FromResult(tempArray);
        }

        /// <summary>
        /// Логика обработки и ограничений по использованию команд.
        /// </summary>
        /// <param name="inputList">Входной массив, состоящий из команды и аргументов.</param>
        /// <param name="telegramUsere">Пользователь из telegram.</param>
        /// <returns>Результат выполнения команды.</returns>
        private async Task<string> HandleCommand(List<string> inputList, User telegramUser, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            if (inputList[0] == "/start")
                return await StartCommand(telegramUser, ct) + "\r\n";

            if (inputList[0] == "/help")
                return helpInfo + "\r\n";

            if (inputList[0] == "/info")
                return version + "\r\n";

            ToDoUser? user = await CheckAuthUser(telegramUser, ct);

            if (inputList[0] == "/addtask")
                return await AddTask(inputList[1], user, ct) + "\r\n";

            if (inputList[0] == "/find")
                return await FindTasks(inputList[1], user, ct) + "\r\n";

            if (inputList[0] == "/showalltasks")
                return await ShowTasks(false, user, ct) + "\r\n";

            if (inputList[0] == "/showtasks")
                return await ShowTasks(true, user, ct) + "\r\n";

            if (inputList[0] == "/removetask")
                return await RemoveTask(inputList, user, ct) + "\r\n";

            if (inputList[0] == "/completetask")
                return await CompleteTask(inputList, user, ct) + "\r\n";

            if (inputList[0] == "/report")
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
        private async Task<string> AddTask(string taskName, ToDoUser user, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            ToDoItem newItem = await toDoService.Add(user, taskName, ct);
            currentCommands.Clear();
            currentCommands.Add(new BotCommand("/addtask", "Добавление задачи."));
            currentCommands.Add(new BotCommand("/showtasks", "Вывод выполненных задач пользователя."));
            currentCommands.Add(new BotCommand("/showalltasks", "Вывод всех задач пользователя."));
            currentCommands.Add(new BotCommand("/removetask", "Удаление задачи."));
            currentCommands.Add(new BotCommand("/completetask", "Выполнить задачу."));
            currentCommands.Add(new BotCommand("/find", "Поиск задач."));
            currentCommands.Add(new BotCommand("/info", "Информация о версии бота."));
            currentCommands.Add(new BotCommand("/help", "Информация о работе с ботом."));
            currentCommands.Add(new BotCommand("/report", "Вывод статистики пользователя."));

            keyboard = new ReplyKeyboardMarkup(
                new[]
                {
                    new KeyboardButton("/showtasks"),
                    new KeyboardButton("/showalltasks"),
                    new KeyboardButton("/report")
                });

            return $"Задача {newItem.Name} добавлена!";
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

            currentCommands.Clear();
            currentCommands.Add(new BotCommand("/addtask", "Добавление задачи."));
            currentCommands.Add(new BotCommand("/info", "Информация о версии бота."));
            currentCommands.Add(new BotCommand("/help", "Информация о работе с ботом."));
            return "Пользователь авторизовался!";
        }

        /// <summary>
        /// Вывод на консоль списка задач. В зависимости от флага выводится разный список задач.
        /// </summary>
        /// <param name="activeStateFlag">true для вывода активных задач, false для вывода всех задач.</param>
        /// <returns>Список задач.</returns>
        private async Task<string> ShowTasks(bool activeStateFlag, ToDoUser user, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            IReadOnlyList<ToDoItem> toDoItems;

            if (activeStateFlag)
                toDoItems = await toDoService.GetActiveByUserId(user.UserId, ct);
            else
                toDoItems = await toDoService.GetAllByUserId(user.UserId, ct);

            return await ToDoListInString(toDoItems, activeStateFlag, ct);
        }

        /// <summary>
        /// Удаление задачи.
        /// </summary>
        /// <param name="inputList">Массив с командой и аргументами.</param>
        /// <returns>Результат удаления задачи.</returns>
        private async Task<string> RemoveTask(List<string> inputList, ToDoUser user, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            var parseInputArg = Guid.Parse(inputList[1]);

            await toDoService.Delete(parseInputArg, ct);

            if ((await toDoService.GetAllByUserId(user.UserId, ct)).Count == 0)
            {
                currentCommands.Clear();
                currentCommands.Add(new BotCommand("/addtask", "Добавление задачи."));
                currentCommands.Add(new BotCommand("/info", "Информация о версии бота."));
                currentCommands.Add(new BotCommand("/help", "Информация о работе с ботом."));
                keyboard = new ReplyKeyboardRemove();
            }
            else
            {
                currentCommands.Clear();
                currentCommands.Add(new BotCommand("/addtask", "Добавление задачи."));
                currentCommands.Add(new BotCommand("/showtasks", "Вывод выполненных задач пользователя."));
                currentCommands.Add(new BotCommand("/showalltasks", "Вывод всех задач пользователя."));
                currentCommands.Add(new BotCommand("/removetask", "Удаление задачи."));
                currentCommands.Add(new BotCommand("/completetask", "Выполнить задачу."));
                currentCommands.Add(new BotCommand("/find", "Поиск задач."));
                currentCommands.Add(new BotCommand("/info", "Информация о версии бота."));
                currentCommands.Add(new BotCommand("/help", "Информация о работе с ботом."));
                currentCommands.Add(new BotCommand("/report", "Вывод статистики пользователя."));
            }

            return "Задача удалена!";
        }

        /// <summary>
        /// Отметка о выполнении задачи.
        /// </summary>
        /// <param name="inputList">Массив с командой и аргументами.</param>
        /// <returns>Результат отметки выполнения задачи.</returns>
        private async Task<string> CompleteTask(List<string> inputList, ToDoUser user, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            var parseInputArg = Guid.Parse(inputList[1]);

            await toDoService.MarkCompleted(parseInputArg, user, ct);

            return "Задача отмечена выполненной!";
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
        private async Task<string> FindTasks(string inputArg, ToDoUser user, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            IReadOnlyList<ToDoItem> toDoItems = await toDoService.Find(user, inputArg, ct);

            //Так как данный список был получен на основании полного списка задач пользователя, можно использовать метод ToDoListInString для вывода задач с информацией о статусе задач.
            return await ToDoListInString(toDoItems, false, ct);
        }

        /// <summary>
        /// Преобразование массива задач в строку для последующего вывода в консоль.
        /// </summary>
        /// <param name="toDoItems">Список задач.</param>
        /// <param name="activeStateFlag">true для вывода активных задач, false для вывода всех задач.</param>
        /// <returns>Сформированная строка.</returns>
        private Task<string> ToDoListInString(IReadOnlyList<ToDoItem> toDoItems, bool activeStateFlag, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            StringBuilder sbResult = new StringBuilder("");
            string tempString;
            int itemsCount = toDoItems.Count;

            for (int i = 0; i < itemsCount; i++)
            {
                tempString = activeStateFlag ? "" : $"({toDoItems[i].State}) ";
                sbResult.Append($"{i + 1}. {tempString}{toDoItems[i].Name} - {toDoItems[i].CreatedAt} - <code>{toDoItems[i].Id}</code>\r\n");
            }

            return Task.FromResult(sbResult.ToString().Trim());
        }
    }
}