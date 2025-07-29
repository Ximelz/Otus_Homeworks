using System.Runtime.CompilerServices;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Otus_Linq_Homework_13
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

                string path = "E:\\Otus homeworks\\ToDoItems";

                IUserRepository userRepository = new FileUsersRepository(path);
                IUserService userService = new UserService(userRepository);
                IToDoRepository toDoRepository = new FileToDoRepository(path);
                IToDoListRepository toDoListRepository = new FileToDoListRepository(path);
                IToDoService toDoService = new ToDoService(maxTasks, maxLengthNameTask, toDoRepository);
                IToDoListService toDoListService = new ToDoListService(toDoListRepository);
                IEnumerable<IScenario> scenarios = new List<IScenario>
                {
                    new AddTaskScenario(toDoService, userService, toDoListService),
                    new FindTaskScenario(toDoService, userService),
                    new RemoveTaskScenario(toDoService, userService),
                    new DeleteListScenario(userService,toDoListService, toDoService),
                    new AddListScenario(userService, toDoListService),
                };
                IScenarioContextRepository contextRepository = new InMemoryScenarioContextRepository();
                IUpdateHandler updateHandler = new UpdateHandler(userService, toDoService, scenarios, contextRepository, toDoListService);
                void DisplayStartEventMessage(string message) => Console.WriteLine($"\r\n�������� ��������� ��������� {message}\r\n");
                void DisplayCompleteEventMessage(string message) => Console.WriteLine($"����������� ��������� ��������� {message}\r\n");

                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = Array.Empty<UpdateType>(),
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

                        Console.WriteLine("������� ���������� \"A\" ��� ������� \"�\" ��� ��������� ����.");
                        var inputKey = Console.ReadKey();

                        if (inputKey.Key == ConsoleKey.A)
                        {
                            ct.Cancel();
                            throw new Exception($"\r\n{me.FirstName} ����������!");
                        }
                        else
                            Console.WriteLine($"\r\n{me.FirstName} �������!");

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
                Console.WriteLine(@"��������� �������������� ������: {0} {1} {2} {3}", ex.GetType(), ex.Message,
                    ex.StackTrace, ex.InnerException);
            }
        }



        /// <summary>
        /// ��������� ������������� ���������� �����.
        /// </summary>
        /// <param name="maxTasksStr">��������� ������������ ���������� ����� � ���� ������.</param>
        /// <returns>��������� ��������� ����������� ������������� ���������� �����.</returns>
        private static int SetMaxTasks()
        {
            Console.WriteLine("������� ����������� ���������� ���������� �����, ������� 1, �������� 100:");
            string? inputString = Console.ReadLine();
            ValidateString(inputString);
            int maxTasks = ParseAndValidateInt(inputString, 1, 100);
            Console.WriteLine($"������������ ���������� ����� ������ {maxTasks}.");
            return maxTasks;
        }

        /// <summary>
        /// ��������� ������������ ����� ������.
        /// </summary>
        /// <param name="maxLenghtNameTasksStr">��������� ������������ ����� ������ � ���� ������.</param>
        /// <returns>��������� ��������� ����������� ������������ ����� ������.</returns>
        private static int SetMaxLengthNameTasks()
        {
            Console.WriteLine("������� ����������� ���������� ����� ������, ������� 1, �������� 100:");
            string? inputString = Console.ReadLine();
            ValidateString(inputString);
            int maxLengthNameTask = ParseAndValidateInt(inputString, 1, 100);
            Console.WriteLine($"������������ ����� ����� ������ ������ {maxLengthNameTask}.");

            return maxLengthNameTask;
        }

        /// <summary>
        /// ����� ��� �������������� ������ � ����� �� ���������� ���������.
        /// </summary>
        /// <param name="str">�������� ������.</param>
        /// <param name="min">������ ������� ���������.</param>
        /// <param name="max">������� ������� ���������.</param>
        /// <returns>��������������� ����� �� ���������� ���������.</returns>
        private static int ParseAndValidateInt(string? str, uint min, uint max)
        {
            int parseInt;
            bool parseFlag = int.TryParse(str, out parseInt);
            if (parseFlag)
                if (parseInt <= max && parseInt >= min)
                    return parseInt;
                else
                    throw new ArgumentOutOfRangeException($"�� ����� \"{parseInt}\", ����� ������� �� ������� ���������� ��������� [{min}:{max}]!");
            throw new ArgumentException($"�� ����� \"{str}\", ��� �� �����!");
        }

        /// <summary>
        /// ����� ��� �������� ������ �� ������� ��� null.
        /// </summary>
        /// <param name="str">�������� ������.</param>
        /// <exception cref="ArgumentException">���� ������ null ��� ������, �� ���������� ����������.</exception>
        private static void ValidateString(string? str)
        {
            if (str == null)
                throw new ArgumentException("�������� ������ ����� �������� \"null\"");

            if (str.Trim() == "")
                throw new ArgumentException("�������� ������ ������");
        }
    }
}
