using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System.Threading;

namespace Otus_Async_Homework_8
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
                    CancellationTokenSource ct = new CancellationTokenSource();
                    void DisplayStartEventMessage(string message) => Console.WriteLine($"\r\nНачалась обработка сообщения {message}\r\n");
                    void DisplayCompleteEventMessage(string message) => Console.WriteLine($"Закончилась обработка сообщения {message}\r\n");

                    try
                    {
                        using (ct = new CancellationTokenSource())
                        {
                            ((UpdateHandler)updateHandler).OnHandleUpdateStarted += DisplayStartEventMessage;
                            ((UpdateHandler)updateHandler).OnHandleUpdateCompleted += DisplayCompleteEventMessage;

                            client.StartReceiving(updateHandler, ct.Token);
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
                    Console.WriteLine(@"Произошла непредвиденная ошибка: {0} {1} {2} {3}", ex.GetType(), ex.Message,
                        ex.StackTrace, ex.InnerException);
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
