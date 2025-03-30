using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 1. Подключение библиотеки Otus.ToDoList.ConsoleBot
 *       1.1 Добавить к себе в решение и в зависимости к своему проекту с ботом проект Otus.ToDoList.ConsoleBot GitHub
 *       1.2 Ознакомиться с классами в папке Types и с README.md
 *       1.3 Создать класс UpdateHandler, который реализует интерфейс IUpdateHandler, и перенести в метод HandleUpdateAsync обработку всех команд. Вместо Console.WriteLine использовать SendMessage у ITelegramBotClient
 *       1.4 Перенести try/catch в HandleUpdateAsync. В Main оставить catch(Exception)
 *       1.5 Для вывода в коноль сообщений использовать метод ITelegramBotClient.SendMessage
 * 2. Удалить команду /echo
 * 3. Изменение логики команды /start
 *       3.1 Не нужно запрашивать имя
 *       3.2 Добавить класс User
 *           3.2.1 Свойства
 *                 3.2.1.1 Guid UserId
 *                 3.2.1.2 long TelegramUserId
 *                 3.2.1.3 string TelegramUserName
 *                 3.2.1.4 DateTime RegisteredAt
 * 4. Добавление класса сервиса UserService
 *       4.1 Добавить интерфейс IUserService
 *       
 *           interface IUserService
 *           {
 *              User RegisterUser(long telegramUserId, string telegramUserName);
 *              User? GetUser(long telegramUserId);
 *           }
 *           
 *       4.2 Создать класс UserService, который реализует интерфейс IUserService. Заполнять telegramUserId и telegramUserName нужно из значений Update.Message.From
 *       4.3 Добавить использование IUserService в UpdateHandler. Получать IUserService нужно через конструктор
 *       4.4 При команде /start нужно вызвать метод IUserService.RegisterUser.
 *       4.5 Если пользователь не зарегистрирован, то ему доступны только команды /help /info
 * 5. Добавление класса ToDoItem
 *       5.1 Добавить enum ToDoItemState с двумя значениями
 *           5.1.1 Active
 *           5.1.2 Completed
 *       5.2 Добавить класс ToDoItem
 *           5.2.1 Свойства
 *           5.2.2 Guid Id
 *           5.2.3 User User
 *           5.2.4 string Name
 *           5.2.5 DateTime CreatedAt
 *           5.2.6 ToDoItemState State
 *           5.2.7 DateTime? StateChangedAt - обновляется при изменении State
 *       5.3 Добавить использование класса ToDoItem вместо хранения только имени задачи
 * 6. Изменение логики /showtasks
 *       6.1 Выводить только задачи с ToDoItemState.Active
 *       6.2 Добавить вывод CreatedAt и Id. Пример: Имя задачи - 01.01.2025 00:00:00 - 17056344-0e03-4a21-b0dd-f0d30a5abf49
 * 7. Добавление класса сервиса ToDoService
 *       7.1 Добавить интерфейс IToDoService
 *           
 *           public interface IToDoService
 *           {
 *              //Возвращает ToDoItem для UserId со статусом Active
 *              IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
 *              ToDoItem Add(User user, string name);
 *              void MarkCompleted(Guid id);
 *              void Delete(Guid id);
 *           }
 *           
 *       7.2 Создать класс ToDoService, который реализует интерфейс IToDoService. Перенести в него логику обработки команд
 *       7.3 Добавить использование IToDoService в UpdateHandler. Получать IToDoService нужно через конструктор
 *       7.4 Изменить формат обработки команды /addtask. Нужно сразу передавать имя задачи в команде. Пример: /addtask Новая задача
 *       7.5 Изменить формат обработки команды /removetask. Нужно сразу передавать номер задачи в команде. Пример: /removetask 2
 * 8. Добавление команды /completetask
 *       8.1 Добавить обработку новой команды /completetask. При обработке команды использовать метод IToDoService.MarkAsCompleted
 *       8.2 Пример: /completetask 73c7940a-ca8c-4327-8a15-9119bffd1d5e
 * 9. Добавление команды /showalltasks
 *       9.1 Добавить обработку новой команды /showalltasks. По ней выводить команды с любым State и добавить State в вывод
 *       9.2 Пример: (Active) Имя задачи - 01.01.2025 00:00:00 - ffbfe448-4b39-4778-98aa-1aed98f7eed8
 * 10. Обновить /help
 */

namespace Otus_Interfaces_Homework_6
{
    /// <summary>
    /// Класс обработки введенных команд.
    /// </summary>
    public class UpdateHandler : IUpdateHandler
    {
        public UpdateHandler(IUserService userService)
        {
            this.userService = userService;
            currentCommands = new string[] { "/start", "/info", "/help" };
            toDoService = new ToDoService();
            removeCommandActive = false;
            completeCommandActive = false;
            startFlag = false;
        }
        public const string helpInfo = "Для работы с ботом нужно ввести команду. В программе существуют следующие команды:\n\r" +
                                       "/info - получите информацию о версии и дате создания и изменения программы.\n\r" +
                                       "/help - получите информацию о работе с программой.\n\r" +
                                       "/start - команда для авторизации пользователя в боте. После авторизации становятся доступны следующие команды:\n\r" +
                                       "/showtasks - команда показывает все активные задачи.\n\r" +
                                       "/showalltasks - команда показывает все имеющиеся задачи.\n\r" +
                                       "/addtask {name} - команда для добавления задачи. В качестве аргумента {name} передается имя задачи.\n\r" +
                                       "/removetask {id} - команда, котороая отмечает выбранную задачу выполненной. В качестве аргумента {id} передается номер задачи, полученный командой \"/showtasks\". " +
                                                          "Команда становится доступной после ввода команды \"/showalltasks\". После ввода любой команды данная команда удаляется из списка доступных команд до следующего ввожда \"/showalltasks\".\n\r" +
                                       "/completetask {id} - команда, котороая отмечает выбранную задачу выполненной. В качестве аргумента {id} передается номер задачи, полученный командой \"/showtasks\". " +
                                                          "Команда становится доступной после ввода команды \"/showtasks\". После ввода любой команды данная команда удаляется из списка доступных команд до следующего ввожда \"/showtasks\".\n\r" +
                                       "Для окончания работы с ботом нужно нажать комбинацию клавиш Ctrl + C.";
        public const string version = "Версия программы 0.4, дата создания 20.02.2025, дата изменения 30.03.2025";

        private string[] currentCommands;           //Список доступных команд.
        private ConsoleUser user;                   //Текущий пользователь.
        private IUserService userService;           //Интерфейс для регистрации пользователя.
        private IToDoService toDoService;           //Интерфейс для взаимодействия с задачами.
        private bool removeCommandActive;           //Флаг доступности команды удаления задачи.
        private bool completeCommandActive;         //Флаг доступности команды выполнения задачи.
        private bool startFlag;                     //Флаг авторизации пользователя для старта бота.

        /// <summary>
        /// Метод обработки обновления задач.
        /// </summary>
        /// <param name="botClient">Текущая сессия бота.</param>
        /// <param name="update">Объект с информацией о введенном сообщении (кто, откуда и что введено).</param>
        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            try
            {
                string[] messageInArray = update.Message.Text.Split(' ');

                switch (messageInArray[0])
                {
                    case ("/start"):
                        botClient.SendMessage(update.Message.Chat, StartCommand(update.Message.From) + "\r\n");
                        break;
                    case ("/help"):
                        botClient.SendMessage(update.Message.Chat, helpInfo + "\r\n");
                        removeCommandActive = false;
                        completeCommandActive = false;
                        break;
                    case ("/info"):
                        botClient.SendMessage(update.Message.Chat, version + "\r\n");
                        removeCommandActive = false;
                        completeCommandActive = false;
                        break;
                    case ("/showalltasks"):
                        string showAllTaskResult;

                        if (!startFlag)
                            showAllTaskResult = "Вы не авторизированный в программе! Используйте команду \"/start\"";
                        else
                            showAllTaskResult = ShowTasks(false) + "\r\n";

                        botClient.SendMessage(update.Message.Chat, showAllTaskResult);
                        break;
                    case ("/showtasks"):
                        string showTaskResult;

                        if (!startFlag)
                            showTaskResult = "Вы не авторизированный в программе! Используйте команду \"/start\"";
                        else
                            showTaskResult = ShowTasks(true) + "\r\n";

                        botClient.SendMessage(update.Message.Chat, showTaskResult);
                        break;
                    case ("/addtask"):
                        string addTaskResult;

                        if (!startFlag)
                            addTaskResult = "Вы не авторизированный в программе! Используйте команду \"/start\"";
                        else if (messageInArray.Length <= 1)
                            addTaskResult = "Вы не ввели аргументы к команде \"/addtask\"!\r\n";
                        else
                        {
                            messageInArray = StringArrayHandler(messageInArray);
                            addTaskResult = AddTask(messageInArray[1]) + "\r\n";
                        }

                        botClient.SendMessage(update.Message.Chat, addTaskResult);
                        break;
                    case ("/removetask"):
                        string removeTaskResult;

                        if (!startFlag)
                            removeTaskResult = "Вы не авторизированный в программе! Используйте команду \"/start\"";
                        else if (messageInArray.Length <= 1)
                            removeTaskResult = "Вы не ввели аргументы к команде \"/removetask\"!\r\n";
                        else
                        {
                            messageInArray = StringArrayHandler(messageInArray);
                            removeTaskResult = RemoveTask(messageInArray[1]) + "\r\n";
                        }

                        botClient.SendMessage(update.Message.Chat, removeTaskResult);
                        break;
                    case ("/completetask"):
                        string completeTaskResult;

                        if (!startFlag)
                            completeTaskResult = "Вы не авторизированный в программе! Используйте команду \"/start\"";
                        else if (messageInArray.Length <= 1)
                            completeTaskResult = "Вы не ввели аргументы к команде \"/completetask\"!\r\n";
                        else
                        {
                            messageInArray = StringArrayHandler(messageInArray);
                            completeTaskResult = CompleteTask(messageInArray[1]) + "\r\n";
                        }

                        botClient.SendMessage(update.Message.Chat, completeTaskResult);
                        completeCommandActive = false;
                        break;
                    default:
                        botClient.SendMessage(update.Message.Chat, "Неверно введена команда\r\n");
                        removeCommandActive = false;
                        completeCommandActive = false;
                        break;
                }
                botClient.SendMessage(update.Message.Chat, "Список доступных команд:");
                foreach (string command in currentCommands)
                {
                    botClient.SendMessage(update.Message.Chat, command);
                }
                botClient.SendMessage(update.Message.Chat, "Введите команду:");
            }
            catch (TaskCountLimitException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
            catch (TaskLengthLimitException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
            catch (DuplicateTaskException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Обработка массива для объединения разбитого по ячейкам аргумента в 1 строку.
        /// </summary>
        /// <param name="inputArray">Входной массив.</param>
        /// <returns>Обработанный массив из 2 элементов.</returns>
        private string[] StringArrayHandler(string[] inputArray)
        {
            string[] tempArray = new string[2];
            int inputArrayLength = inputArray.Length;

            tempArray[0] = inputArray[0];

            for(int i = 1; i < inputArrayLength; i++)
                tempArray[1] += inputArray[i] + " ";

            tempArray[1] = tempArray[1].Trim();

            return tempArray;
        }

        /// <summary>
        /// Команда добавления задачи.
        /// </summary>
        /// <param name="taskName">Имя задачи.</param>
        /// <returns>Результат добавление задачи.</returns>
        public string AddTask(string taskName)
        {
            if (taskName == "" || taskName == null)
                return "Вы не ввели имя задачи!";

            ToDoItem newTask;
            newTask = toDoService.Add(user, taskName);
            currentCommands = new string[] { "/showtasks", "/showalltasks", "/addtask", "/info", "/help" };

            removeCommandActive = false;
            completeCommandActive = false;

            return $"Задача {newTask.Name} добавлена!";
        }

        /// <summary>
        /// Удаление задачи.
        /// </summary>
        /// <param name="removeNumber">Номер задачи на удаление.</param>
        /// <returns>Результат удаления задачи.</returns>
        public string RemoveTask(string removeNumber)
        {
            if (removeCommandActive)
                return "Команда \"/removetask\" доступна только после ввода команды \"/showalltasks\"";

            IReadOnlyList<ToDoItem> toDoItems = new List<ToDoItem>();
            toDoItems = toDoService.GetAllByUserID(user.UserId);

            if (!int.TryParse(removeNumber, out int i))
                return "Введено не число!";

            if (i > toDoService.GetTasksCount())
                return "Введен неверный номер задачи!";

            toDoService.Delete(toDoItems[i - 1].Id);

            if (toDoService.GetTasksCount() == 0)
                currentCommands = new string[] { "/addtask", "/info", "/help" };

            removeCommandActive = false;
            return "Задача удалена!";
        }

        /// <summary>
        /// Авторизация и старт работы программы.
        /// </summary>
        /// <param name="telegramUser">Пользователь из telegram.</param>
        /// <returns>Результат авторизации пользователя</returns>
        private string StartCommand(User telegramUser)
        {
            user = userService.RegisterUser(telegramUser.Id, telegramUser.Username);
            if (user == null)
                return "Текущий пользователь не смог авторизоваться!";
            currentCommands = new string[] { "/addtask", "/info", "/help" };
            startFlag = true;
            return "Пользователь авторизовался!";
        }

        /// <summary>
        /// Вывод на консоль списка задач. В зависимости от флага выводится разный список задач.
        /// </summary>
        /// <param name="activeStateFlag">true для вывода активных задач, false для вывода всех задач.</param>
        /// <returns>Список задач.</returns>
        private string ShowTasks(bool activeStateFlag)
        {
            removeCommandActive = false;
            completeCommandActive = false;

            IReadOnlyList<ToDoItem> toDoItems = new List<ToDoItem>();
            string result = "";

            if (activeStateFlag)
                toDoItems = toDoService.GetActiveByUserID(user.UserId);
            else
                toDoItems = toDoService.GetAllByUserID(user.UserId);

            int itemsCount = toDoItems.Count;

            if (activeStateFlag)
            {
                for (int i = 0; i < itemsCount; i++)
                    result += $"{i + 1}. {toDoItems[i].Name} - {toDoItems[i].CreatedAt} - {toDoItems[i].Id}\r\n";

                removeCommandActive = true;
                currentCommands = new string[] { "/showtasks", "/showalltasks", "/addtask", "/completetask", "/info", "/help" };
            }
            else
            {
                for (int i = 0; i < itemsCount; i++)
                    result += $"{i + 1}. ({toDoItems[i].State}) {toDoItems[i].Name} - {toDoItems[i].CreatedAt} - {toDoItems[i].Id}\r\n";

                completeCommandActive = true;
                currentCommands = new string[] { "/showtasks", "/showalltasks", "/addtask", "/removetask", "/info", "/help" };
            }
            

            return result;
        }

        /// <summary>
        /// Отметка о выполнении задачи.
        /// </summary>
        /// <param name="removeNumber">Номер выполненной задачи.</param>
        /// <returns>Результат отметки выполения задачи.</returns>
        private string CompleteTask(string removeNumber)
        {
            if (completeCommandActive)
                return "Команда \"/completetask\" доступна только после ввода команды \"/showtasks\"";

            IReadOnlyList<ToDoItem> toDoItems = new List<ToDoItem>();
            toDoItems = toDoService.GetAllByUserID(user.UserId);

            if (!int.TryParse(removeNumber, out int i))
                return "Введено не число!";

            if (i > toDoService.GetTasksCount())
                return "Введен неверный номер задачи!";

            currentCommands = new string[] { "/showtasks", "/showalltasks", "/addtask", "/info", "/help" };

            toDoService.MarkCompleted(toDoItems[i - 1].Id);
            completeCommandActive = false;
            return "Задача отмечена выполненной!";
        }
    }
    class TaskCountLimitException : Exception
    {
        /// <summary>
        /// Исключение при добавлении задачи, выходящей за пределы максимально допустимого количества задач.
        /// </summary>
        /// <param name="taskCountLimit">Максимальное количество задач.</param>
        public TaskCountLimitException(int taskCountLimit) : base($"Превышено максимальное количество задач равное \"{taskCountLimit}\"") { }
    }

    class TaskLengthLimitException : Exception
    {
        /// <summary>
        /// Исключение при вводе слишком длинного описания задачи.
        /// </summary>
        /// <param name="taskLength">Длина описания текущей задачи.</param>
        /// <param name="taskLengthLimit">Максимальная длина описания задачи.</param>
        public TaskLengthLimitException(int taskLength, int taskLengthLimit) : base($"Длина задачи \"{taskLength}\" превышает максимально допустимое значение \"{taskLengthLimit}\"") { }
    }

    class DuplicateTaskException : Exception
    {
        /// <summary>
        /// Исключение при вводе дубля задачи.
        /// </summary>
        /// <param name="task">Введенная задача.</param>
        public DuplicateTaskException(string task) : base($"Задача \"{task}\" уже существует!") { }
    }
}