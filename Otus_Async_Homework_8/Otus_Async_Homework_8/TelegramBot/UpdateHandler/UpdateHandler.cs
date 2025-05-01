using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Async_Homework_8
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
            currentCommands = new string[] { "/start", "/info", "/help" };
            OnHandleUpdateStarted += DisplayEventMessage;
            OnHandleUpdateCompleted += DisplayEventMessage;
        }

        ~UpdateHandler()
        {
            OnHandleUpdateStarted -= DisplayEventMessage;
            OnHandleUpdateCompleted -= DisplayEventMessage;
        }

        public const string helpInfo = "Для работы с ботом нужно ввести команду. В программе существуют следующие команды:\n\r" +
                                       "/info - получите информацию о версии и дате создания и изменения программы.\n\r" +
                                       "/help - получите информацию о работе с программой.\n\r" +
                                       "/start - команда для авторизации пользователя в боте. После авторизации становятся доступны следующие команды:\n\r" +
                                       "/showtasks - команда показывает все активные задачи.\n\r" +
                                       "/showalltasks - команда показывает все имеющиеся задачи.\n\r" +
                                       "/addtask {name} - команда для добавления задачи. В качестве аргумента {name} передается имя задачи.\n\r" +
                                       "/removetask {id} {stateFlag} - команда, котороая отмечает выбранную задачу выполненной. В качестве аргумента {id} передается номер задачи, полученный командой \"/showtasks\". " +
                                       "Аргумент {stateFlag} определяет из какого списка берется номер задачи для удаления. {stateFlag} может иметь только 2 значения: \"all\" и \"active\".\n\r" +
                                       "/completetask {id} {stateFlag} - команда, котороая отмечает выбранную задачу выполненной. В качестве аргумента {id} передается номер задачи, полученный командой \"/showtasks\"." +
                                       "Аргумент {stateFlag} определяет из какого списка берется номер задачи для изменения. {stateFlag} может иметь только 2 значения: \"all\" и \"active\".\n\r" +
                                       "/report - команда для отображение статистики по задачам текущего пользователя.\n\r" +
                                       "/find {str} - команда для поиска задачи с префиксом {str} текущего пользователя.\n\r" +
                                       "Для окончания работы с ботом нужно нажать комбинацию клавиш Ctrl + C.";
        public const string version = "Версия программы 0.7, дата создания 20.02.2025, дата изменения 30.04.2025";

        private string[] currentCommands;                                   //Список доступных команд.
        private readonly IUserService userService;                          //Интерфейс для регистрации пользователя.
        private readonly IToDoService toDoService;                          //Интерфейс для взаимодействия с задачами.
        private delegate void MessageEventHandler(string message);          //Делегат для событий.
        private event MessageEventHandler? OnHandleUpdateStarted;           //Событие начала обработки введенного сообщения.
        private event MessageEventHandler? OnHandleUpdateCompleted;         //Событие конца обработки введенного сообщения.

        /// <summary>
        /// Метод для вывода события в консоль.
        /// </summary>
        /// <param name="message">Сообщение события.</param>
        private void DisplayEventMessage(string message) => Console.WriteLine(message);

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
                OnHandleUpdateStarted?.Invoke($"\r\nНачалась обработка сообщения {update.Message.Text}\r\n");

                List<string> messageInList = StringArrayHandler(update.Message.Text);
                botClient.SendMessage(update.Message.Chat, HeandlerCommands(messageInList, update.Message.From, ct), ct);

                OnHandleUpdateCompleted?.Invoke($"Закончилась обработка сообщения {update.Message.Text}\r\n");
            }
            finally
            {
                botClient.SendMessage(update.Message.Chat, BotsCommandString(currentCommands), ct);
            }

        }

        /// <summary>
        /// Метод вывода ошибки в консоль.
        /// </summary>
        /// <param name="botClient">Клиент телеграмм бота.</param>
        /// <param name="exception">Исключение.</param>
        /// <param name="ct">Объект отмены задачи.</param>
        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                ct.ThrowIfCancellationRequested();

            Console.WriteLine(exception.Message);
        }

        /// <summary>
        /// Обработка аргументов введенной строки.
        /// </summary>
        /// <param name="message">Введенная строка.</param>
        /// <returns>Обработанный массив команды с аргументами.</returns>
        private List<string> StringArrayHandler(string message)
        {
            List<string> inputList = DeleteNullItemsArray(new List<string>(message.Split(' ')));

            if (!currentCommands.Contains(inputList[0]))
                throw new Exception("Введена неверная команда!");

            if (inputList[0] == "/addtask" || inputList[0] == "/find")
                if (inputList.Count > 1)
                    return ConcatArgsInArray(inputList);
                else
                    throw new Exception($"Введено неверное количество аргументов для команды {inputList[0]}.");

            if (inputList[0] == "/removetask" || inputList[0] == "/completetask")
            {
                if (inputList.Count != 3)
                    throw new Exception($"Введено неверное количество аргументов для команды {inputList[0]}. Их должно быть 3!");
                return inputList;
            }

            if (inputList.Count == 1)
                return inputList;
                        
            throw new Exception($"Введено неверное количество аргументов для команды {inputList[0]}.");
        }

        /// <summary>
        /// Удаление пустых элементов списка.
        /// </summary>
        /// <param name="inputList">Входной список.</param>
        /// <returns>Отформатированный список.</returns>
        private List<string> DeleteNullItemsArray(List<string> inputList)
        {
            List<string> tempList = new List<string>();

            foreach (string item in inputList)
                if (ValidateString(item))
                    tempList.Add(item);

            return tempList;
        }

        /// <summary>
        /// Проверка строки на пустоту или null.
        /// </summary>
        /// <param name="item">Входная строка.</param>
        /// <returns>true если строка не пустая, false если строка null, пустая или состоит из пробелов.</returns>
        private bool ValidateString(string item)
        {
            if (item == null)
                return false;

            if (item.Trim() == "")
                return false;

            return true;
        }

        /// <summary>
        /// Объединение всех элементов списка начиная со 2 элемента.
        /// </summary>
        /// <param name="inputList">Входной список.</param>
        /// <returns>Объединенный список из 2-х элементов.</returns>
        private List<string> ConcatArgsInArray(List<string> inputList)
        {
            List<string> tempArray = new List<string>();
            int inputArrayLength = inputList.Count;
            var tempsb = new StringBuilder("");

            tempArray.Add(inputList[0]);

            for (int i = 1; i < inputArrayLength; i++)
                tempsb.Append(inputList[i] + " ");

            tempArray.Add(tempsb.ToString().Trim());

            return tempArray;
        }

        /// <summary>
        /// Логика обработки и ограничений по использованию команд.
        /// </summary>
        /// <param name="inputList">Входной массив, состоящий из команды и аргументов.</param>
        /// <param name="telegramUsere">Пользователь из telegram.</param>
        /// <returns>Результат выполнения команды.</returns>
        private string HeandlerCommands(List<string> inputList, User telegramUser, CancellationToken ct)
        {
            //Доп условие на проверку наличия команды в списке доступных команд нужно для того, чтобы во время выполнения программы не использовал недоступные команды.
            if (inputList[0] == "/start" && currentCommands.Contains("/start"))
                return StartCommand(telegramUser, ct) + "\r\n";

            if (inputList[0] == "/help")
                return helpInfo + "\r\n";

            if (inputList[0] == "/info")
                return version + "\r\n";

            ToDoUser user = CheckAuthUser(telegramUser, ct);

            //Проверка старта программы за счет получение информации об авторизации пользователя. 
            if (user == null)
            {
                currentCommands = new string[] { "/start", "/info", "/help" };
                throw new Exception("Вы не авторизированны в программе! Используйте команду \"/start\" для авторизации.");
            }

            if (inputList[0] == "/addtask")
                return AddTask(inputList[1], user, ct) + "\r\n";

            if (inputList[0] == "/find")
                return FindTasks(inputList[1], user, ct) + "\r\n";

            if (inputList[0] == "/showalltasks" && currentCommands.Contains("/showalltasks"))
                return ShowTasks(false, user, ct) + "\r\n";

            if (inputList[0] == "/showtasks" && currentCommands.Contains("/showtasks"))
                return ShowTasks(true, user, ct) + "\r\n";

            if (inputList[0] == "/removetask" && currentCommands.Contains("/removetask"))
                return RemoveTask(inputList, user, ct) + "\r\n";

            if (inputList[0] == "/completetask" && currentCommands.Contains("/completetask"))
                return CompleteTask(inputList, user, ct) + "\r\n";

            if (inputList[0] == "/report" && currentCommands.Contains("/report"))
                return ReportUserTasks(user, ct) + "\r\n";

            throw new Exception("Введена неверная команда!");
        }

        /// <summary>
        /// Проверка на авторизацию поьзователя.
        /// </summary>
        /// <param name="telegramUser">Пользователь telegram.</param>
        /// <returns>Возвращает объект ConsoleUser если авторизирован, null если нет.</returns>
        private ToDoUser CheckAuthUser(User telegramUser, CancellationToken ct)
        {
            ToDoUser user = userService.GetUser(telegramUser.Id, ct).Result;
            if (user != null)
                return user;
            
            return null;
        }

        /// <summary>
        /// Команда добавления задачи.
        /// </summary>
        /// <param name="taskName">Имя задачи.</param>
        /// <returns>Результат добавление задачи.</returns>
        private string AddTask(string taskName, ToDoUser user, CancellationToken ct)
        {
            ToDoItem newItem = toDoService.Add(user, taskName, ct).Result;
            currentCommands = new string[] { "/showtasks", "/showalltasks", "/addtask", "/removetask", "/completetask", "/find", "/info", "/help", "/report"};
            return $"Задача {newItem.Name} добавлена!";
        }

        /// <summary>
        /// Авторизация и старт работы программы.
        /// </summary>
        /// <param name="telegramUser">Пользователь из telegram.</param>
        /// <returns>Результат авторизации пользователя</returns>
        private string StartCommand(User telegramUser, CancellationToken ct)
        {
            ToDoUser user = userService.RegisterUser(telegramUser.Id, telegramUser.Username, ct).Result;

            if (user == null)
                throw new Exception("Текущий пользователь не смог авторизоваться!");

            currentCommands = new string[] { "/addtask", "/info", "/help" };
            return "Пользователь авторизовался!";
        }

        /// <summary>
        /// Вывод на консоль списка задач. В зависимости от флага выводится разный список задач.
        /// </summary>
        /// <param name="activeStateFlag">true для вывода активных задач, false для вывода всех задач.</param>
        /// <returns>Список задач.</returns>
        private string ShowTasks(bool activeStateFlag, ToDoUser user, CancellationToken ct)
        {
            IReadOnlyList<ToDoItem> toDoItems;

            if (activeStateFlag)
                toDoItems = toDoService.GetActiveByUserId(user.UserId, ct).Result;
            else
                toDoItems = toDoService.GetAllByUserId(user.UserId, ct).Result;

            return ToDoListInString(toDoItems, activeStateFlag);
        }

        /// <summary>
        /// Удаление задачи.
        /// </summary>
        /// <param name="inputList">Массив с командой и аргументами.</param>
        /// <returns>Результат удаления задачи.</returns>
        private string RemoveTask(List<string> inputList, ToDoUser user, CancellationToken ct)
        {
            IReadOnlyList<ToDoItem> tasks = GetToDoItemsList(toDoService, inputList[2], user, ct);

            int tasksCount = tasks.Count;
            int parseInputArg = ParseInt(inputList[1]);

            if (tasksCount < parseInputArg)
                throw new Exception($"Введенный номер задачи \"{inputList[1]}\" выходит за пределы количества из указанного списка задач \"{tasksCount}\"");

            toDoService.Delete(tasks[parseInputArg - 1].Id, ct);

            if (toDoService.GetAllByUserId(user.UserId, ct).Result.Count == 0)
                currentCommands = new string[] { "/addtask", "/info", "/help" };
            else
                currentCommands = new string[] { "/showtasks", "/showalltasks", "/addtask", "/removetask", "/completetask", "/find", "/info", "/help", "/report" };

            return "Задача удалена!";
        }

        /// <summary>
        /// Отметка о выполнении задачи.
        /// </summary>
        /// <param name="inputList">Массив с командой и аргументами.</param>
        /// <returns>Результат отметки выполения задачи.</returns>
        private string CompleteTask(List<string> inputList, ToDoUser user, CancellationToken ct)
        {
            IReadOnlyList<ToDoItem> tasks = GetToDoItemsList(toDoService, inputList[2], user, ct);

            int tasksCount = tasks.Count;
            int parseInputArg = ParseInt(inputList[1]);

            if (tasksCount < parseInputArg)
                throw new Exception($"Введенный номер задачи \"{inputList[1]}\" выходит за пределы количества из указанного списка задач \"{tasksCount}\"");

            toDoService.MarkCompleted(tasks[parseInputArg - 1].Id, user, ct);

            return "Задача отмечена выполненной!";
        }

        /// <summary>
        /// Вывод статистики пользователя по задачам.
        /// </summary>
        /// <param name="user">Пользователь, чьи задачи ищем.</param>
        /// <returns>Статистика задач пользователя.</returns>
        private string ReportUserTasks(ToDoUser user, CancellationToken ct)
        {
            IToDoReportService reportService = new ToDoReportService(toDoService);
            var (total, completed, active, generatedAt) = reportService.GetUserStats(user.UserId, ct).Result;

            return $"Статистика по задачам на {generatedAt}. Всего: {total}; Завершено: {completed}; Активных: {active}";
        }

        /// <summary>
        /// Поиск задач по аргументу.
        /// </summary>
        /// <param name="inputList">Команда с аргументом.</param>
        /// <param name="user">Пользователь, чьи задачи ищем.</param>
        /// <returns>Список задач.</returns>
        private string FindTasks(string inputArg, ToDoUser user, CancellationToken ct)
        {
            IReadOnlyList<ToDoItem> toDoItems = toDoService.Find(user, inputArg, ct).Result;

            //Так как данный список был получен на основании полного списка задач пользователя, можно использовать метод ToDoListInString для вывода задач с информацией о статусе задач.
            return ToDoListInString(toDoItems, false);
        }

        /// <summary>
        /// Преобразование строки в число.
        /// </summary>
        /// <param name="str">Входная строка.</param>
        /// <returns>Преобразованное число.</returns>
        private int ParseInt(string str)
        {
            if(int.TryParse(str, out int parseInt))
                return parseInt;

            throw new Exception($"Вы ввели \"{str}\", это не число!");
        }

        /// <summary>
        /// Формирование строки из массива для подготовки к выводу на экран списка доступных команд.
        /// </summary>
        /// <param name="inputArray">Входной массив.</param>
        /// <returns>Строка со списком команд.</returns>
        private string BotsCommandString(string[] inputArray)
        {
            string resultString = "Список доступных команд:\r\n";
            foreach (string command in inputArray)
                resultString += command + "\r\n";
            resultString += "Введите команду:";
            return resultString.TrimEnd();
        }

        /// <summary>
        /// Преобразование массива задач в строку для последующего вывода в консоль.
        /// </summary>
        /// <param name="toDoItems">Список задач.</param>
        /// <param name="activeStateFlag">true для вывода активных задач, false для вывода всех задач.</param>
        /// <returns>Сформированная строка.</returns>
        private string ToDoListInString(IReadOnlyList<ToDoItem> toDoItems, bool activeStateFlag)
        {
            StringBuilder sbResult = new StringBuilder("");
            string tempString;
            int itemsCount = toDoItems.Count;

            for (int i = 0; i < itemsCount; i++)
            {
                tempString = activeStateFlag ? "" : $"({toDoItems[i].State}) ";
                sbResult.Append($"{i + 1}. {tempString}{toDoItems[i].Name} - {toDoItems[i].CreatedAt} - {toDoItems[i].Id}\r\n");
            }

            return sbResult.ToString().Trim();
        }

        /// <summary>
        /// Получение списка задач в зависимости от введенного аргумента.
        /// </summary>
        /// <param name="toDoService">Объект для работы с задачами.</param>
        /// <param name="modeArg">Введенный аргумент.</param>
        /// <returns>Список задач.</returns>
        /// <exception cref="ArgsException">Ошибка если неверно введен аргумент.</exception>
        private IReadOnlyList<ToDoItem> GetToDoItemsList(IToDoService toDoService, string modeArg, ToDoUser user, CancellationToken ct)
        {
            if (modeArg.Trim() == "all")
                return toDoService.GetAllByUserId(user.UserId, ct).Result;

            if (modeArg.Trim() == "active")
                return toDoService.GetActiveByUserId(user.UserId, ct).Result;
            
            throw new Exception($"Введен неверный 3 ({modeArg}) аргумент!");
        }
    }
}