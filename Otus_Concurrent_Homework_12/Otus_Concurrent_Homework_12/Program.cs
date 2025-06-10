using System.Runtime.CompilerServices;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Otus_Concurrent_Homework_12
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                int maxTasks = SetMaxTasks();
                int maxLengthNameTask = SetMaxLengthNameTasks();

                string _botKey = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN", EnvironmentVariableTarget.User);
                var bot = new TelegramBotClient(_botKey);

                string path = "E:\\Otus homeworks 10\\ToDoItems";

                IUserRepository userRepository = new FileUsersRepository(path);
                IUserService userService = new UserService(userRepository);
                IToDoRepository toDoRepository = new FileToDoRepository(path);
                IToDoService toDoService = new ToDoService(maxTasks, maxLengthNameTask, toDoRepository);
                IEnumerable<IScenario> scenarios = new List<IScenario>
                {
                    new AddTaskScenario(toDoService, userService),
                    new FindTaskScenario(toDoService, userService),
                    new CompleteTaskScenario(toDoService, userService),
                    new RemoveTaskScenario(toDoService, userService)
                };
                IScenarioContextRepository contextRepository = new InMemoryScenarioContextRepository();
                IUpdateHandler updateHandler = new UpdateHandler(userService, toDoService, scenarios, contextRepository);
                void DisplayStartEventMessage(string message) => Console.WriteLine($"\r\nНачалась обработка сообщения {message}\r\n");
                void DisplayCompleteEventMessage(string message) => Console.WriteLine($"Закончилась обработка сообщения {message}\r\n");

                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = [UpdateType.Message],
                    DropPendingUpdates = true
                };

                try
                {
                    using (CancellationTokenSource ct = new CancellationTokenSource())
                    {
                        var me = await bot.GetMe();

                        ((UpdateHandler)updateHandler).OnHandleUpdateStarted += DisplayStartEventMessage;
                        ((UpdateHandler)updateHandler).OnHandleUpdateCompleted += DisplayCompleteEventMessage;

                        bot.StartReceiving(updateHandler, receiverOptions, ct.Token);

                        Console.WriteLine("Нажмите английскую \"A\" или русскую \"Ф\" для остановки бота.");
                        var inputKey = Console.ReadKey();

                        if (inputKey.Key == ConsoleKey.A)
                        {
                            ct.Cancel();
                            throw new Exception($"\r\n{me.FirstName} остановлен!");
                        }
                        else
                            Console.WriteLine($"\r\n{me.FirstName} запущен!");

                        await Task.Delay(-1);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    ((UpdateHandler)updateHandler).OnHandleUpdateStarted -= DisplayStartEventMessage;
                    ((UpdateHandler)updateHandler).OnHandleUpdateCompleted -= DisplayCompleteEventMessage;
                }
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
                Console.WriteLine(@"Произошла непредвиденная ошибка: {0} {1} {2} {3}", ex.GetType(), ex.Message,
                    ex.StackTrace, ex.InnerException);
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
            string? inputString = Console.ReadLine();
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
            string? inputString = Console.ReadLine();
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
