using System.Xml.Serialization;

/*
 * ДЗ №4. Исключения.
 * 1.Добавить глобальный try catch
 *      1.1 Добавьте try catch в метод Main +
 *      1.2 catch должен отлавливать все виды исключений и выводить в консоль сообщение “Произошла непредвиденная ошибка: “ с информацией об исключении (Type, Message, StackTrace, InnerException) +
 *      1.3 Попадание в catch не должно останавливать работу приложения +/-
 * 2.Добавить ограничение на максимальное количество задач
 *      2.1 При старте приложения выводите текст «Введите максимально допустимое количество задач» +
 *      2.2 Ожидайте ввод из консоли. Это должно быть число от 1 до 100, иначе нужно выбросить исключение ArgumentException с сообщением. +
 *      2.3 В методе Main добавьте отдельный catch для типа ArgumentException и в нем выводите в консоль только сообщение из исключения. +
 *      2.4 Создайте свой тип исключения TaskCountLimitException, который в конструкторе должен принимать только int taskCountLimit, а сообщение должно быть вида $“Превышено максимальное количество задач равное {taskCountLimit}“ +
 *      2.5 Добавьте проверку на максимально допустимое количество задач в обработчик команды /addtask. Если количество превышено, то нужно выбросить исключение TaskCountLimitException.
 *      2.6 В методе Main добавьте отдельный catch для типа TaskCountLimitException и в нем выводите в консоль только сообщение из исключения. +
 * 3.Добавить ограничение на максимальную длину задачи
 *      3.1 При старте приложения выводите текст «Введите максимально допустимую длину задачи» +
 *      3.2 Ожидайте ввод из консоли. Это должно быть число от 1 до 100, иначе нужно выбросить исключение ArgumentException с сообщением. +
 *      3.3 Создайте свой тип исключения TaskLengthLimitException, который в конструкторе должен принимать int taskLength, int taskLengthLimit, а сообщение должно быть вида $“Длина задачи ‘{taskLength}’ превышает максимально допустимое значение {taskLengthLimit}“. +
 *      3.4 Добавьте проверку на максимально допустимую длину задачи в обработчик команды /addtask. Если длина превышена, то нужно выбросить исключение TaskLengthLimitException.
 *      3.5 В методе Main добавьте отдельный catch для типа TaskLengthLimitException и в нем выводите в консоль только сообщение из исключения. +
 * 4.Добавить проверку на дубликаты задач
 *      4.1 Создайте свой тип исключения DuplicateTaskException, который в конструкторе должен принимать string task, а сообщение должно быть вида $“Задача ‘{task}’ уже существует“.
 *      4.2 Добавьте проверку на дубликаты задач в обработчик команды /addtask. Если пользователь пытается добавить уже существующую задачу., то нужно выбросить исключение DuplicateTaskException.
 *      4.3 В методе Main добавьте отдельный catch для типа DuplicateTaskExceptionи в нем выводите в консоль только сообщение из исключения. +
 * 5.Добавить метод int ParseAndValidateInt(string? str, int min, int max), который приводит полученную строку к int и проверяет, что оно находится в диапазоне min и max. В противном случае выбрасывать ArgumentException с сообщением. Добавить использование этого метода в приложение. +
 * 6.Добавить метод void ValidateString(string? str), который проверяет, что строка не равна null, не равна пустой строке и имеет какие-то символы кроме проблема. В противном случае выбрасывать ArgumentException с сообщением. Добавить использование этого метода в приложение. +
 * 7.Вынести обработчики команд в отдельные методы +
 */

namespace Otus_Homework_4
{
    internal class Program
    {
        public const string helpInfo = "Для работы с программой выберите с помощью стрелочек на клавиатуре команду и нажмите Enter. В программе существуют следующие команды:\n\r" +
                                       "/info - получите информацию о версии и дате создания и изменения программы.\n\r" +
                                       "/help - получите информацию о работе с программой.\n\r" +
                                       "/start - команда для начала работы с программой. После ее выбора необходимо введите свое имя.\n\r" +
                                       "/echo - команда для вывода введенной строки. После ее выбора необходимо ввести выводимую строку.\n\r" +
                                       "/return - команда для возвращает вас на предыдущее меню. Вместо выбора этой команды можно использовать клавишу ESC.\n\r" +
                                       "/showTask - команда показывает имеющиеся задачи. При ее выборе выводятся имеющиеся задачи и для управления ими добавляются 2 команды /addTask и /removeTask.\n\r" +
                                       "/addTask - команда для добавления задачи. После ввода задачи программа предлагает выбрать следующие команды:/yes, /no и /return (в данном случае команда будет возвращать пользователя в меню отображения имеющихся задач).\n\r" +
                                       "/removeTask - команда для удаления задачи. После ввода задачи программа предлагает выбрать задачу на удаление или вернуться в предыдущее меню (/return), а потом либо подтвердить ее удаление (/yes), либо вернуться к выбору задачи на удаление (/return).\n\r" +
                                       "/yes - команда подтверждает добавление/удаление задачи.\n\r" +
                                       "/no - команда отменяет создание задачи и предлагает пользователю еще раз ввести задачу.\n\r" +
                                       "/exit - команда заканчивает работу с программой.";
        public const string version = "Версия программы 0.3, дата создания 20.02.2025, дата изменения 14.03.2025";
        public string name = "";
        public List<string> menuOptions;
        public List<string> tasks = new List<string>();
        public int maxTasks = 0;
        public int maxLengthTaskString = 0;

        static void Main(string[] args)
        {
            var prog = new Program();       //Данное объявление нужно из-за возникновения ошибки CS0120. Решение ошибки взято из документации .NET.
            while (true)                            //п.1.3???
            {
                try                                 //п.1.1
                {
                    prog.ConsoleBot();
                    break;
                }
                catch (TaskCountLimitException ex)  //п.2.6
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadKey();
                }
                catch (TaskLengthLimitException ex) //п.3.5
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadKey();
                }
                catch (ArgumentException ex)        //п.2.3
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadKey();
                }
                catch (DuplicateTaskException ex)   //п.4.3
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadKey();
                }
                catch (Exception ex)                //п.1.1 и п.1.2
                {
                    Console.WriteLine(@"Произошла непредвиденная ошибка: {0} {1} {2} {3}", ex.GetType(), ex.Message, ex.StackTrace, ex.InnerException);
                    Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// Метод работы консольного бота для работы с задачами пользователя.
        /// </summary>
        private void ConsoleBot()
        {
            if (maxTasks == 0)                     //п.2.1 и п.2.2
            {
                Console.WriteLine("Введите максимально допустимое количество задач, минимум 1, максимум 100:");
                string inputString = Console.ReadLine();
                ValidateString(inputString);        //п.6
                maxTasks = ParseAndValidateInt(inputString, 1, 100);
            }

            if (maxLengthTaskString == 0)          //п.3.1 и п.3.2
            {
                Console.WriteLine("Введите максимально допустимую длину задачи, минимум 1, максимум 100:");
                string inputString = Console.ReadLine();
                ValidateString(inputString);        //п.6
                maxLengthTaskString = ParseAndValidateInt(inputString, 1, 100);
            }

            if (name == "")
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
                    case ConsoleKey.Escape:
                        command = "/return";
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

            if (tasks.Count == maxTasks)                                   //п.2.5
                throw new TaskCountLimitException(maxTasks);

            string inputString;
            while (true)
            {
                Console.WriteLine("Введите задачу:");
                inputString = Console.ReadLine().Trim();

                ValidateString(inputString);
                //if (inputString == "")
                //{
                //    Console.Clear();
                //    Console.WriteLine("Вы ничего не ввели!");
                //    continue;
                //}

                if (inputString.Length > maxLengthTaskString)             //п.3.3
                    throw new TaskLengthLimitException(inputString.Length, maxLengthTaskString);

                if (tasks.Contains(inputString))                          //п.4.2
                    throw new DuplicateTaskException(inputString);

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

        /// <summary>
        /// Метод для преобразования строки в число из указанного диапазона.
        /// </summary>
        /// <param name="str">Входящая строка.</param>
        /// <param name="min">Нижняя граница диапазона.</param>
        /// <param name="max">Верхняя граница диапазона.</param>
        /// <returns>Преобразованное число из введенного диапазона.</returns>
        /// <exception cref="ArgumentException">Введена неверная строка.</exception>
        private int ParseAndValidateInt(string? str, int min, int max)          //п.5
        {
            int parseInt;
            bool parseFlag = int.TryParse(str, out parseInt);
            Console.Clear();
            if (parseFlag)
                if (parseInt <= max && parseInt >= min)
                    return parseInt;
                else
                    throw new ArgumentException($"Вы ввели \"{parseInt}\", число выходит за пределы указанного диапазона [{min}:{max}]!");
            throw new ArgumentException($"Вы ввели \"{str}\", это не число!");
        }

        /// <summary>
        /// Метод для проверки строки на пустоту или null.
        /// </summary>
        /// <param name="str">Входящая строка.</param>
        /// <exception cref="ArgumentException">Если строка null или пустая, то вызывается исключение.</exception>
        private void ValidateString(string? str)                                //п.6
        {
            if (str == null)
                throw new ArgumentException("Введеная строка имеет значение \"null\"");

            if (str.Trim() == "")
                throw new ArgumentException("Введеная строка пустая");
        }
    }

    class TaskCountLimitException : Exception           //п.2.4
    {
        public string Message;

        /// <summary>
        /// Исключение при добавлении задачи, выходящей за пределы максимально допустимого количества задач.
        /// </summary>
        /// <param name="taskCountLimit">Максимальное количество задач.</param>
        public TaskCountLimitException(int taskCountLimit)
        {
            Message = $"Превышено максимальное количество задач равное \"{taskCountLimit}\"";
        }
    }

    class TaskLengthLimitException : Exception          //п.3.3
    {
        public string Message;

        /// <summary>
        /// Исключение при вводе слишком длинного описания задачи.
        /// </summary>
        /// <param name="taskLength">Длина описания текущей задачи.</param>
        /// <param name="taskLengthLimit">Максимальная длина описания задачи.</param>
        public TaskLengthLimitException(int taskLength, int taskLengthLimit)
        {
            Message = $"Длина задачи \"{taskLength}\" превышает максимально допустимое значение \"{taskLengthLimit}\"";
        }
    }

    class DuplicateTaskException : Exception            //п.4.1
    {
        public string Message;

        /// <summary>
        /// Исключение при вводе дубля задачи.
        /// </summary>
        /// <param name="task">Введенная задача.</param>
        public DuplicateTaskException(string task)
        {
            Message = $"Задача \"{task}\" уже существует!";
        }

    }
}
