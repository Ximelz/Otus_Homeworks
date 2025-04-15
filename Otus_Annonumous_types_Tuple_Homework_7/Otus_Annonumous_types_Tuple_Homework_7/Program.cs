using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using Otus_Annonumous_types_Tuple_Homework_7;

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
    public class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    int maxTasks = SetMaxTasks();
                    int maxLengthNameTask = SetMaxLengthNameTasks();

                    ConsoleBotClient client = new ConsoleBotClient();
                    IUserRepository userRepository = new InMemoryUserRepository();
                    IUserService userService = new UserService(userRepository);
                    IToDoRepository toDoRepository = new InMemoryToDoRepository();
                    IToDoService toDoService = new ToDoService(maxTasks, maxLengthNameTask, toDoRepository);
                    IUpdateHandler updateHandler = new UpdateHandler(userService, toDoService);

                    client.StartReceiving(updateHandler);

                    break;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(@"Произошла непредвиденная ошибка: {0} {1} {2} {3}", ex.GetType(), ex.Message, ex.StackTrace, ex.InnerException);
                    Console.ReadKey();
                }
            }
        }



        /// <summary>
        /// Установка максимального количества задач.
        /// </summary>
        /// <param name="maxTasksStr">Введенное максимальное количество задач в виде строки.</param>
        /// <returns>Результат установки ограничения максимального количества задач.</returns>
        private static int SetMaxTasks()
        {
            Console.WriteLine("Введите максимально допустимое количество задач, минимум 1, максимум 100:");
            string inputString = Console.ReadLine();
            ValidateString(inputString);
            int maxTasks = ParseAndValidateInt(inputString, 1, 100);
            Console.WriteLine($"Максимальное количество задач теперь {maxTasks}.");
            return maxTasks;
        }

        /// <summary>
        /// Установка максимальной длины задачи.
        /// </summary>
        /// <param name="maxLenghtNameTasksStr">Введенная максимальная длина задачи в виде строки.</param>
        /// <returns>Результат установки ограничения максимальной длины задачи.</returns>
        private static int SetMaxLengthNameTasks()
        {
            Console.WriteLine("Введите максимально допустимую длину задачи, минимум 1, максимум 100:");
            string inputString = Console.ReadLine();
            ValidateString(inputString);
            int maxLengthNameTask = ParseAndValidateInt(inputString, 1, 100);
            Console.WriteLine($"Максимальная длина имени задачи теперь {maxLengthNameTask}.");

            return maxLengthNameTask;
        }

        /// <summary>
        /// Метод для преобразования строки в число из указанного диапазона.
        /// </summary>
        /// <param name="str">Входящая строка.</param>
        /// <param name="min">Нижняя граница диапазона.</param>
        /// <param name="max">Верхняя граница диапазона.</param>
        /// <returns>Преобразованное число из введенного диапазона.</returns>
        private static int ParseAndValidateInt(string? str, uint min, uint max)
        {
            int parseInt;
            bool parseFlag = int.TryParse(str, out parseInt);
            if (parseFlag)
                if (parseInt <= max && parseInt >= min)
                    return parseInt;
                else
                    throw new ArgumentOutOfRangeException($"Вы ввели \"{parseInt}\", число выходит за пределы указанного диапазона [{min}:{max}]!");
            throw new ArgumentException($"Вы ввели \"{str}\", это не число!");
        }

        /// <summary>
        /// Метод для проверки строки на пустоту или null.
        /// </summary>
        /// <param name="str">Входящая строка.</param>
        /// <exception cref="ArgumentException">Если строка null или пустая, то вызывается исключение.</exception>
        private static void ValidateString(string? str)
        {
            if (str == null)
                throw new ArgumentException("Введеная строка имеет значение \"null\"");

            if (str.Trim() == "")
                throw new ArgumentException("Введеная строка пустая");
        }
    }
}
