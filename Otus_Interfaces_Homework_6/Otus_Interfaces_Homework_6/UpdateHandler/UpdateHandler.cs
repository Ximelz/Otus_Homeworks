using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using Otus_Interfaces_Homework_6;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
        public UpdateHandler(IUserService userService, IToDoService iToDoService)
        {
            this.userService = userService;
            this.iToDoService = iToDoService;
            currentCommands = new string[] { "/start", "/info", "/help" };
        }
        public const string helpInfo = "Для работы с ботом нужно ввести команду. В программе существуют следующие команды:\n\r" +
                                       "/info - получите информацию о версии и дате создания и изменения программы.\n\r" +
                                       "/help - получите информацию о работе с программой.\n\r" +
                                       "/start - команда для авторизации пользователя в боте. После авторизации становятся доступны следующие команды:\n\r" +
                                       "/showtasks - команда показывает все активные задачи.\n\r" +
                                       "/showalltasks - команда показывает все имеющиеся задачи.\n\r" +
                                       "/maxtasks - команда для указания максимального количества задач. Может быть только в диапазоне от 1 до 100." +
                                                          "Доступна после команды \"/start\" и перестает быть доступной после ввода максимального количества задач и максимально имени задачи." +
                                       "/maxnametask - команда для указания максимального имени задачи. Может быть только в диапазоне от 1 до 100." +
                                                          "Доступна после команды \"/start\" и перестает быть доступной после ввода максимального количества задач и максимально имени задачи." +
                                       "/addtask {name} - команда для добавления задачи. В качестве аргумента {name} передается имя задачи.\n\r" +
                                       "/removetask {id} {stateFlag} - команда, котороая отмечает выбранную задачу выполненной. В качестве аргумента {id} передается номер задачи, полученный командой \"/showtasks\". " +
                                                          "Аргумент {stateFlag} определяет из какого списка берется номер задачи для удаления. {stateFlag} может иметь только 2 значения: \"all\" и \"active\".\n\r" +
                                       "/completetask {id} {stateFlag} - команда, котороая отмечает выбранную задачу выполненной. В качестве аргумента {id} передается номер задачи, полученный командой \"/showtasks\"." +
                                                          "Аргумент {stateFlag} определяет из какого списка берется номер задачи для изменения. {stateFlag} может иметь только 2 значения: \"all\" и \"active\".\n\r" +
                                       "Для окончания работы с ботом нужно нажать комбинацию клавиш Ctrl + C.";
        public const string version = "Версия программы 0.5, дата создания 20.02.2025, дата изменения 02.04.2025";

        private string[] currentCommands;                     //Список доступных команд.
        private ConsoleUser user;                             //Текущий пользователь.
        private readonly IUserService userService;            //Интерфейс для регистрации пользователя.
        private readonly IToDoService iToDoService;           //Интерфейс для взаимодействия с задачами.
        private int maxTasks;                                 //Максимальное количество задач.
        private int maxLengthNameTask;

        /// <summary>
        /// Метод обработки обновления задач.
        /// </summary>
        /// <param name="botClient">Текущая сессия бота.</param>
        /// <param name="update">Объект с информацией о введенном сообщении (кто, откуда и что введено).</param>
        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            try
            {
                List<string> messageInList = StringArrayHandler(update.Message.Text);
                botClient.SendMessage(update.Message.Chat, HeandlerCommands(messageInList, update.Message.From));
            }
            catch (TaskCountLimitException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (TaskLengthLimitException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (DuplicateTaskException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (AuthUserException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ArgsException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (CommandException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (IndexOutRange ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                botClient.SendMessage(update.Message.Chat, BotsCommandString(currentCommands));
            }
        }

        /// <summary>
        /// Обработка аргументов введенной строки.
        /// </summary>
        /// <param name="message">Введенная строка..</param>
        /// <returns>Обработанный массив команды с аргументами.</returns>
        private List<string> StringArrayHandler(string message)
        {
            List<string> inputList = DeleteNullItemsArray(new List<string>(message.Split(' ')));

            if (!currentCommands.Contains(inputList[0]))
                throw new CommandException("Введена неверная команда!");

            if (inputList[0] == "/addtask")
                if (inputList.Count > 1)
                    return ConcatArgsInArray(inputList);
                else
                    throw new ArgsException($"Введено неверное количество аргументов для команды {inputList[0]}.");

            if (inputList[0] == "/maxtasks" || inputList[0] == "/maxnametask")
                {
                    if (inputList.Count != 2)
                        throw new ArgsException($"Введено неверное количество аргументов для команды {inputList[0]}. Их должно быть 2!");
                    return inputList;
                }

            if (inputList[0] == "/removetask" || inputList[0] == "/completetask")
            {
                if (inputList.Count != 3)
                    throw new ArgsException($"Введено неверное количество аргументов для команды {inputList[0]}. Их должно быть 3!");
                return inputList;
            }

            if (inputList.Count == 1)
                return inputList;
                        
            throw new ArgsException($"Введено неверное количество аргументов для команды {inputList[0]}.");
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
        private string HeandlerCommands(List<string> inputList, User telegramUser)
        {
            //Доп условие на проверку наличия команды в списке доступных команд нужно для того, чтобы во время выполнения программы не использовал недоступные команды.
            if (inputList[0] == "/start" && currentCommands.Contains("/start"))
                return StartCommand(telegramUser) + "\r\n";

            if (inputList[0] == "/help")
                return helpInfo + "\r\n";

            if (inputList[0] == "/info")
                return version + "\r\n";


            //Проверка старта программы за счет получение информации об авторизации пользователя. 
            if (!CheckAuthUser(telegramUser))
            {
                currentCommands = new string[] { "/start", "/info", "/help" };
                throw new AuthUserException("Вы не авторизированны в программе! Используйте команду \"/start\" для авторизации.");
            }


            if (inputList[0] == "/maxtasks" && currentCommands.Contains("/maxtasks"))
                return SetMaxTasks(inputList[1]) + "\r\n";

            if (inputList[0] == "/maxnametask" && currentCommands.Contains("/maxnametask"))
                return SetMaxLengthNameTasks(inputList[1]) + "\r\n";


            //Прежде чем работать с задачами нужно ввести максимальное их количество.
            if (maxTasks == 0)
                throw new ArgumentException("Вы не ввели максимальное количество задач!");

            //Прежде чем работать с задачами нужно ввести максимальную длину имени задачи.
            if (maxLengthNameTask == 0)
                throw new ArgumentException("Вы не ввели максимальную длину задачи!");


            if (inputList[0] == "/addtask" && currentCommands.Contains("/addtask"))
                return AddTask(inputList[1]) + "\r\n";

            if (inputList[0] == "/showalltasks" && currentCommands.Contains("/showalltasks"))
                return ShowTasks(false) + "\r\n";

            if (inputList[0] == "/showtasks" && currentCommands.Contains("/showtasks"))
                return ShowTasks(true) + "\r\n";

            if (inputList[0] == "/removetask" && currentCommands.Contains("/removetask"))
                return RemoveTask(inputList) + "\r\n";

            if (inputList[0] == "/completetask" && currentCommands.Contains("/completetask"))
                return CompleteTask(inputList) + "\r\n";

            throw new CommandException("Введена неверная команда!");
        }

        /// <summary>
        /// Проверка на авторизацию поьзователя.
        /// </summary>
        /// <param name="telegramUser">Пользователь telegram.</param>
        /// <returns>true если авторизирован, false если нет.</returns>
        private bool CheckAuthUser(User telegramUser)
        {
            if (userService.GetUser(telegramUser.Id) == user)
                return true;
            
            return false;
        }

        /// <summary>
        /// Команда добавления задачи.
        /// </summary>
        /// <param name="taskName">Имя задачи.</param>
        /// <returns>Результат добавление задачи.</returns>
        private string AddTask(string taskName)
        {
            //Проверка на случай если команда прошла мимо всех ограничений.
            if (MaxTasksCheck())
                throw new TaskCountLimitException(maxTasks);

            if (taskName.Length > maxLengthNameTask)
                throw new TaskLengthLimitException(taskName.Length, maxLengthNameTask);

            if (!DublicateCheck(taskName))
                throw new DuplicateTaskException(taskName);

            ToDoItem newItem = iToDoService.Add(user, taskName);

            //Если достигнуто максимальное количество задач, то команда /addtask удаляется из доступных команд.
            if (MaxTasksCheck())
                currentCommands = new string[] { "/showtasks", "/showalltasks", "/removetask", "/completetask", "/info", "/help" };
            else
                currentCommands = new string[] { "/showtasks", "/showalltasks", "/addtask", "/removetask", "/completetask", "/info", "/help" };

            return $"Задача {newItem.Name} добавлена!";
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
                throw new AuthUserException("Текущий пользователь не смог авторизоваться!");

            currentCommands = new string[] { "/maxnametask", "/maxtasks", "/info", "/help" };
            return "Пользователь авторизовался!";
        }

        /// <summary>
        /// Вывод на консоль списка задач. В зависимости от флага выводится разный список задач.
        /// </summary>
        /// <param name="activeStateFlag">true для вывода активных задач, false для вывода всех задач.</param>
        /// <returns>Список задач.</returns>
        private string ShowTasks(bool activeStateFlag)
        {
            ToDoService toDoService = ParseToDoService(iToDoService);

            IReadOnlyList<ToDoItem> toDoItems;

            if (activeStateFlag)
                toDoItems = toDoService.GetActiveByUserID(user.UserId);
            else
                toDoItems = toDoService.tasks;

            return ToDoListInString(toDoItems, activeStateFlag);
        }

        /// <summary>
        /// Удаление задачи.
        /// </summary>
        /// <param name="inputList">Массив с командой и аргументами.</param>
        /// <returns>Результат удаления задачи.</returns>
        private string RemoveTask(List<string> inputList)
        {
            ToDoService toDoService = ParseToDoService(iToDoService);
            IReadOnlyList<ToDoItem> tasks = GetToDoItemsList(toDoService, inputList[2]);

            int tasksCount = tasks.Count;
            int parseInputArg = ParseInt(inputList[1]);

            if (tasksCount < parseInputArg)
                throw new IndexOutRange($"Введенный номер задачи \"{inputList[1]}\" выходит за пределы количества из указанного списка задач \"{tasksCount}\"");

            toDoService.Delete(tasks[parseInputArg - 1].Id);

            if (toDoService.tasksCount == 0)
                currentCommands = new string[] { "/addtask", "/info", "/help" };
            else
                currentCommands = new string[] { "/showtasks", "/showalltasks", "/addtask", "/removetask", "/completetask", "/info", "/help" };

                return "Задача удалена!";
        }

        /// <summary>
        /// Отметка о выполнении задачи.
        /// </summary>
        /// <param name="inputList">Массив с командой и аргументами.</param>
        /// <returns>Результат отметки выполения задачи.</returns>
        private string CompleteTask(List<string> inputList)
        {
            ToDoService toDoService = ParseToDoService(iToDoService);
            IReadOnlyList<ToDoItem> tasks = GetToDoItemsList(toDoService, inputList[2]);


            int tasksCount = tasks.Count;
            int parseInputArg = ParseInt(inputList[1]);

            if (tasksCount < parseInputArg)
                throw new IndexOutRange($"Введенный номер задачи \"{inputList[1]}\" выходит за пределы количества из указанного списка задач \"{tasksCount}\"");

            toDoService.MarkCompleted(tasks[parseInputArg - 1].Id);
            tasks[parseInputArg - 1].StateChangedAt = DateTime.Now;

            return "Задача отмечена выполненной!";
        }

        /// <summary>
        /// Метод для преобразования строки в число из указанного диапазона.
        /// </summary>
        /// <param name="str">Входящая строка.</param>
        /// <param name="min">Нижняя граница диапазона.</param>
        /// <param name="max">Верхняя граница диапазона.</param>
        /// <returns>Преобразованное число из введенного диапазона.</returns>
        private int ParseAndValidateInt(string? str, int min, int max)
        {
            int parseInt;
            bool parseFlag = int.TryParse(str, out parseInt);
            if (parseFlag)
                if (parseInt <= max && parseInt >= min)
                    return parseInt;
                else
                    throw new ArgumentException($"Вы ввели \"{parseInt}\", число выходит за пределы указанного диапазона [{min}:{max}]!");
            throw new ArgumentException($"Вы ввели \"{str}\", это не число!");
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

            throw new ArgumentException($"Вы ввели \"{str}\", это не число!");
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
        /// Установка максимального количества задач.
        /// </summary>
        /// <param name="maxTasksStr">Введенное максимальное количество задач в виде строки.</param>
        /// <returns>Результат установки ограничения максимального количества задач.</returns>
        private string SetMaxTasks(string maxTasksStr)
        {
            maxTasks = ParseAndValidateInt(maxTasksStr, 1, 100);

            if (maxLengthNameTask > 0)
                currentCommands = new string[] { "/addtask", "/info", "/help" };

            return $"Максимальное количество задач теперь {maxTasks}.";
        }

        /// <summary>
        /// Установка максимальной длины задачи.
        /// </summary>
        /// <param name="maxLenghtNameTasksStr">Введенная максимальная длина задачи в виде строки.</param>
        /// <returns>Результат установки ограничения максимальной длины задачи.</returns>
        private string SetMaxLengthNameTasks(string maxLenghtNameTasksStr)
        {
            maxLengthNameTask = ParseAndValidateInt(maxLenghtNameTasksStr, 1, 100);

            if (maxTasks > 0)
                currentCommands = new string[] { "/addtask", "/info", "/help" };

            return $"Максимальная длина имени задачи теперь {maxLengthNameTask}.";
        }

        /// <summary>
        /// Проверка на дубль задачи.
        /// </summary>
        /// <param name="name">Имя новой задачи.</param>
        /// <returns>true если такой задачи нет, false если найден дубль.</returns>
        private bool DublicateCheck(string name)
        {
            ToDoService toDoService = ParseToDoService(iToDoService);

            IReadOnlyList<ToDoItem> tasks = toDoService.tasks;

            foreach (ToDoItem toDoItem in tasks)
                if (toDoItem.Name == name)
                    return false;

            return true;
        }

        /// <summary>
        /// Проверка на превышение количества задач.
        /// </summary>
        /// <returns>true если количество задач достигло максимума, false если нет.</returns>
        private bool MaxTasksCheck()
        {
            ToDoService toDoService = ParseToDoService(iToDoService);
            return toDoService.tasksCount >= maxTasks;
        }

        /// <summary>
        /// Получение списка задач в зависимости от введенного аргумента.
        /// </summary>
        /// <param name="toDoService">Объект для работы с задачами.</param>
        /// <param name="modeArg">Введенный аргумент.</param>
        /// <returns>Список задач.</returns>
        /// <exception cref="ArgsException">Ошибка если неверно введен аргумент.</exception>
        private IReadOnlyList<ToDoItem> GetToDoItemsList(ToDoService toDoService, string modeArg)
        {
            if (modeArg.Trim() == "all")
                return toDoService.tasks;
            else if (modeArg.Trim() == "active")
                return toDoService.GetActiveByUserID(user.UserId);
            else
                throw new ArgsException($"Введен неверный 3 ({modeArg}) агрумент!");
        }

        /// <summary>
        /// Приведение IToDoService к ToDoService.
        /// </summary>
        /// <param name="ToDoService">Объект интерфейса.</param>
        /// <returns>Класс, реализующий IToDoService.</returns>
        /// <exception cref="Exception">Ошибка при приобразовании.</exception>
        private ToDoService ParseToDoService(IToDoService ToDoService)
        {
            if (!(ToDoService is ToDoService))
                throw new Exception("Не удалось привести интерфейс \"IToDoService\" к \"ToDoService\"");

            return ToDoService as ToDoService;
        }
    }
}