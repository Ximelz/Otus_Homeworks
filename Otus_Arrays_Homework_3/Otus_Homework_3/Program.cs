using System.Xml.Serialization;

namespace Otus_Homework_2
{
    internal class Program
    {
        public const string helpInfo = "Для работы с программой выберите с помощью стрелочек на клавиатуре команду и нажмите Enter. В программе существуют следующие команды:\n\r" +
                                       "/info - получите информацию о версии и дате создания и изменения программы.\n\r" +
                                       "/help - получите информацию о работе с программой.\n\r" +
                                       "/start - команда для начала работы с программой. После ее выбора необходимо введите свое имя.\n\r" +
                                       "/echo - команда для вывода введенной строки. После ее выбора необходимо ввести выводимую строку.\n\r" +
                                       "/return - команда для возвращает вас на предыдущее меню.\n\r" +
                                       "/showTask - команда показывает имеющиеся задачи. При ее выборе выводятся имеющиеся задачи и для управления ими добавляются 2 команды /addTask и /removeTask.\n\r" +
                                       "/addTask - команда для добавления задачи. После ввода задачи программа предлагает выбрать следующие команды:/yes, /no и /return (в данном случае команда будет возвращать пользователя в меню отображения имеющихся задач).\n\r" +
                                       "/removeTask - команда для удаления задачи. После ввода задачи программа предлагает выбрать задачу на удаление или вернуться в предыдущее меню (/return), а потом либо подтвердить ее удаление (/yes), либо вернуться к выбору задачи на удаление (/return).\n\r" +
                                       "/yes - команда подтверждает добавление/удаление задачи.\n\r" +
                                       "/no - команда отменяет создание задачи и предлагает пользователю еще раз ввести задачу.\n\r" +
                                       "/exit - команда заканчивает работу с программой.";
        public const string version = "Версия программы 0.2, дата создания 20.02.2025, дата изменения 12.03.2025";
        public string name = "";
        public List<string> menuOptions;
        public List<string> tasks = new List<string>();

        static void Main(string[] args)
        {
            var prog = new Program();       //Данное объявление нужно из-за возникновения ошибки CS0120. Решение ошибки взято из документации .NET.
            prog.ConsoleBot();
        }

        /// <summary>
        /// Метод работы консольного бота для работы с задачами пользователя.
        /// </summary>
        private void ConsoleBot()
        {
            menuOptions = new List<string>() { "/info", "/help", "/start", "/exit" };
            while (true)
            {
                Console.SetWindowSize(160,50);
                Console.Clear();
                Console.WriteLine(@"Приветствую{0}. Выберите команду с помощью клавиши Enter!", name == "" ? "" : " " + name);
                PrintMenu(menuOptions);
                int startPos = 1;
                string command;
                command = GetCommand(startPos, menuOptions.Count + startPos - 1, menuOptions);
                if (command == "/exit")
                {
                    Console.Clear();
                    Console.WriteLine("\r\nДо свидания!");
                    break;
                }
                HandlerCommands(command);
            }
        }

        /// <summary>
        /// Получение команды из консоли.
        /// </summary>
        /// <param name="startPos">Стартовая строка консоли с командами.</param>
        /// <param name="lastPos">Конечная строка консоли с командами.</param>
        /// <param name="commands">Список команд.</param>
        /// <returns>Выбранная команда.</returns>
        private string GetCommand(int startPos, int lastPos, List<string> commands) 
        {
            ConsoleKeyInfo inputKey;
            string command = "";
            int currentPos = startPos;
            PrintCursor(currentPos);
            do
            {
                inputKey = Console.ReadKey();
                Console.SetWindowSize(160, 50);                                         //Данный вызов функции нужен для корректного вычисления позиции курсора и отображения команд.
                DeleteCursor(currentPos);
                switch (inputKey.Key)
                {
                    case ConsoleKey.Enter:
                        command = commands[currentPos - startPos];
                        break;
                    case ConsoleKey.UpArrow:
                        currentPos = SetCursorUp(startPos, currentPos, lastPos);
                        break;
                    case ConsoleKey.DownArrow:
                        currentPos = SetCursorDown(startPos, currentPos, lastPos);
                        break;
                }
                PrintCursor(currentPos);

            } while (command == "");
            return command;
        }

        /// <summary>
        /// Метод обработки выбранных команд.
        /// </summary>
        /// <param name="command">Выбранная команда.</param>
        private void HandlerCommands(string command)
        {
            switch (command)
            {
                case ("/start"):
                    StartCommand();
                    break;
                case ("/help"):
                    HelpCommand();
                    break;
                case ("/info"):
                    InfoCommand();
                    break;
                case ("/echo"):
                    EchoCommand();
                    break;
                case ("/showTasks"):
                    ShowTasksCommand();
                    break;
            }

        }

        /// <summary>
        /// Метод для запуска работы программы.
        /// </summary>
        private void StartCommand()
        {
            Console.Clear();
            while (true)
            {
                Console.WriteLine("Введите ваше имя:");
                name = Console.ReadLine().Trim();
                Console.Clear();
                if (name == "")
                {
                    Console.WriteLine("Вы ничего не ввели!");
                    continue;
                }
                Console.Clear();
                Console.WriteLine($"Приветствую {name}");
                Console.WriteLine(" /return");
                string command = GetCommand(1, 1, new List<string>() { "/return" });
                menuOptions = new List<string>{ "/info", "/help", "/echo", "/showTasks", "/exit"};
                break;
            }
        }

        /// <summary>
        /// Метод вывода справки на консоль.
        /// </summary>
        private void HelpCommand()
        {
            Console.Clear();
            Console.WriteLine(helpInfo);
            Console.WriteLine(" /return");
            (int, int) c = Console.GetCursorPosition();
            string command = GetCommand(c.Item2 - 1, c.Item2 - 1, new List<string>() { "/return" });
        }

        /// <summary>
        /// Команда вывода информации о версии и дате создания и изменения программы.
        /// </summary>
        private void InfoCommand()
        {
            Console.Clear();
            Console.WriteLine(version);
            Console.WriteLine(" /return");
            string command = GetCommand(1, 1, new List<string>() { "/return" });
        }

        /// <summary>
        /// Команда вывода введенной строки на консоль.
        /// </summary>
        private void EchoCommand()
        {
            string inputString;
            Console.Clear();
            while (true)
            {
                Console.WriteLine("Введите строку:");
                inputString = Console.ReadLine().Trim();
                if (inputString == "")
                {
                    Console.Clear();
                    Console.WriteLine("Вы ввели пустую строку!");
                    continue;
                }
                Console.Clear();
                Console.WriteLine($"Вы ввели: {inputString}");
                Console.WriteLine(" /return");
                (int, int) c = Console.GetCursorPosition();
                string command = GetCommand(c.Item2 - 1, c.Item2 - 1, new List<string>() { "/return" });
                break;
            }
        }

        /// <summary>
        /// Метод отображения всех имеющихся задач.
        /// </summary>
        private void ShowTasksCommand()
        {
            Console.Clear();
            List<string> taskCommands = new List<string>() { "/addTask", "/removeTask", "/return"};
            string command;
            while (true)
            {
                if (tasks.Count > 0)
                {
                    PrintTasks(taskCommands);
                    (int, int) c = Console.GetCursorPosition();
                    command = GetCommand(c.Item2 - 3, c.Item2 - 1, taskCommands);
                }
                else
                {
                    Console.WriteLine("Нет еще ниодной задачи!\n\r /addTask\n\r /return");
                    command = GetCommand(1, 2, new List<string>() { "/addTask", "/return" });
                }

                switch (command)
                    {
                    case "/addTask":
                        AddTaskCommand();
                        break;
                    case "/removeTask":
                        RemoveTaskCommand();
                        break;
                    case "/return":
                        return;
                    }
                Console.Clear();
            }
        }

        /// <summary>
        /// Метод добавления новой задачи.
        /// </summary>
        private void AddTaskCommand()
        {
            Console.Clear();
            string inputString;
            while (true)
            {
                Console.WriteLine("Введите задачу:");
                inputString = Console.ReadLine().Trim();
                if (inputString == "")
                {
                    Console.Clear();
                    Console.WriteLine("Вы ничего не ввели!");
                    continue;
                }

                if (VerificationAddTask(inputString))
                    return;
                Console.Clear();
            }
        }


        /// <summary>
        /// Метод удаления задачи.
        /// </summary>
        private void RemoveTaskCommand()
        {
            List<string> removeList  = new List<string>();
            removeList.AddRange(tasks);
            removeList.Add("/return");
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Выберите задачу для удаления:");
                foreach (string task in tasks)
                    Console.WriteLine($" {task}");
                Console.WriteLine(" /return");

                (int, int) c = Console.GetCursorPosition();
                string removeTask = GetCommand(1, c.Item2 - 1, removeList);
                if (removeTask == "/return")
                    return;

                Console.Clear();
                Console.WriteLine($"Вы действительно хотите удалить задачу \"{removeTask}\"?");
                Console.WriteLine(" /yes\r\n /return");
                c = Console.GetCursorPosition();
                string command = GetCommand(c.Item2 - 2, c.Item2 - 1, new List<string> { "/yes", "/return" });
                
                switch (command)
                    {
                    case "/yes":
                        tasks.Remove(removeTask);
                        return;
                    }
            }

        }

        /// <summary>
        /// Метод подтверждения добавления задачи в список задач.
        /// </summary>
        /// <param name="task">Текущая задача.</param>
        /// <returns>Результат выполнения программы. true если задача была добавлена.</returns>
        private bool VerificationAddTask(string task)
        {
            string command;
            Console.Clear();
            Console.WriteLine("Вы подтверждаете добавление следующей задачи?");
            Console.WriteLine(task);
            Console.WriteLine(" /yes\r\n /no\r\n /return");
            (int, int) c = Console.GetCursorPosition();
            command = GetCommand(c.Item2 - 3, c.Item2 - 1, new List<string>() {"/yes", "/no", "/return" });
            switch (command)
            {
                case "/no":
                    return false;
                case "/yes":
                    tasks.Add(task);
                    return true;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Метод вывода на консоль команд управления.
        /// </summary>
        /// <param name="commands">Команды управления.</param>
        private void PrintMenu(List<string> commands)
        {
            foreach (string command in commands)
                Console.WriteLine($" {command}");
        }

        /// <summary>
        /// Метод вывода на консоль текущих задач и команд управления.
        /// </summary>
        /// <param name="commands">Команды управления.</param>
        private void PrintTasks(List<string> commands)
        {
            Console.WriteLine("Ваши задачи:");
            int i = 1;
            foreach (string task in tasks)
            {
                Console.WriteLine($"{i}.{task}");
                i++;
            }
            Console.WriteLine();
            PrintMenu(commands);
        }
        
        /// <summary>
        /// Метод добавления курсора в строку.
        /// </summary>
        /// <param name="currentPos">Текущая позиция курсора.</param>
        private void PrintCursor(int currentPos)
        {
            Console.SetCursorPosition(0, currentPos);
            Console.Write(">");
            Console.SetCursorPosition(0, currentPos);
        }
        
        /// <summary>
        /// Метод удаления курсора из строки.
        /// </summary>
        /// <param name="currentPos">Текущая позиция курсора.</param>
        private void DeleteCursor(int currentPos)
        {
            Console.SetCursorPosition(0, currentPos);
            Console.Write(" ");
            Console.SetCursorPosition(0, currentPos);
        }
        
        /// <summary>
        /// Метод перемещения курсора вверх.
        /// </summary>
        /// <param name="startPos">Стартовая позиция команд управления.</param>
        /// <param name="currentPos">Текущая позиция курсора.</param>
        /// <param name="lastPos">Последняя позиция команд управления.</param>
        /// <returns>Новая позиция курсора.</returns>
        private int SetCursorUp(int startPos, int currentPos, int lastPos)
        {
            if (currentPos == startPos)
                return lastPos;
            else
                return --currentPos;
        }
        
        /// <summary>
        /// Метод перемещения курсора вниз.
        /// </summary>
        /// <param name="startPos">Стартовая позиция команд управления.</param>
        /// <param name="currentPos">Текущая позиция курсора.</param>
        /// <param name="lastPos">Последняя позиция команд управления.</param>
        /// <returns>Новая позиция курсора.</returns>
        private int SetCursorDown(int startPos, int currentPos, int lastPos)
        {
            if (currentPos == lastPos)
                return startPos;
            else
                return ++currentPos;
        }
    }
}
