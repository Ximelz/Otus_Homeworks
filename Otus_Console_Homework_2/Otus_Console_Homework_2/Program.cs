namespace Otus_Console_Homework_2
{
    internal class Program
    {
        public const string helpInfo = "\r\nДля работы с программой введите команду в консольную строку. В программе существуют следующие команды:\n\r" +
                                       "1. /info - получите информацию о версии и дате создания программы.\n\r" +
                                       "2. /help - получите информацию о работе с программой.\n\r" +
                                       "3. /start Name - введите чтобы начать работать с программой. В качестве аргумента через пробел введите свое имя.\n\r" +
                                       "4. /echo args - введите команду /echo и аргумент после нее через пробел. Данный агрумент будет выведен на консоль.\n\r" +
                                       "5. /exit - команда заканчивает работу с программой.\n\r";
        public const string version = "\r\nВерсия программы 0.1, дата создания 20.02.2025\n\r";
        public string commandList = "Список доступных команд /start, /help, /info, /exit";
        public string name;
        public bool startFlag = false;

        static void Main(string[] args)
        {
            string command;
            var prog = new Program();       //Данное объявление нужно из-за возникновения ошибки CS0120. Решение ошибки взято из документации .NET.
            Console.WriteLine("Приветствую.");
            while (true)
            {
                Console.WriteLine($"Введите команду.\n\r{prog.commandList}\"");
                command = Console.ReadLine().Trim();
                if (command == "/exit")
                {
                    Console.WriteLine("\r\nДо свидания!");
                    break;
                }
                prog.HandlerCommands(command);
            }
        }

        private void HandlerCommands(string command)
        {
            string[] argsArray = command.Split();
            switch (argsArray[0])
            {
                case ("/start"):
                    if (!startFlag)
                    {
                        name = GetArgs(command);
                        if (name.Length == 0)
                        {
                            Console.WriteLine("\r\nВы не ввели ваше имя!\r\n");
                            break;
                        }
                        startFlag = true;
                        commandList = "Список доступных команд /echo, /help, /info, /exit";
                        Console.WriteLine($"\r\nПриветствую {name}\r\n");
                        break;
                    }
                    Console.WriteLine("\r\nПрограмма уже запущена!\r\n");
                    break;
                case ("/help"):
                    Console.WriteLine(helpInfo);
                    break;
                case ("/info"):
                    Console.WriteLine(version);
                    break;
                case ("/echo"):
                    if (!startFlag)
                        Console.WriteLine("\r\nПрограмма не запущена. Используейте сначала команду /start и введите ваше имя!\r\n");
                    Console.WriteLine($"\r\n{name},с помощтю команды /echo вы ввели {GetArgs(command)}\r\n");
                    break;
                default:
                    Console.WriteLine("\r\nВведена неверная команда!\r\n");
                    break;
            }

        }

        private string GetArgs(string args)
        {
            string[] argsArray = args.Split();
            int arrayLength = argsArray.Length;
            string resultString = "";

            //Первый элемент массива не берется, так как там находится консольная команда.
            for (int i = 1; i < arrayLength; i++)
            {
                resultString += argsArray[i] + " ";
            }
            return resultString.Trim();
        }
    }
}
