using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Interfaces_Homework_6
{
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

    class AuthUserException : Exception
    {
        /// <summary>
        /// Исключение при отсутствии аутентификации пользователя.
        /// </summary>
        /// <param name="exceptionString">Описание ошибки.</param>
        public AuthUserException(string exceptionString) : base(exceptionString) { }
    }

    class ArgsException : Exception
    {
        /// <summary>
        /// Исключение при ошибках с аргументами.
        /// </summary>
        /// <param name="exceptionString">Описание ошибки.</param>
        public ArgsException(string exceptionString) : base(exceptionString) { }
    }

    class CommandException : Exception
    {
        /// <summary>
        /// Исключение при ошибках с вводом команды.
        /// </summary>
        /// <param name="exceptionString">Описание ошибки.</param>
        public CommandException(string exceptionString) : base(exceptionString) { }
    }

    class IndexOutRange : Exception
    {
        /// <summary>
        /// Исключение при удалении или отметки о выполнении задачи, связанное с неправильно введенным номером задачи.
        /// </summary>
        /// <param name="exceptionString">Описание ошибки.</param>
        public IndexOutRange(string exceptionString) : base(exceptionString) { }
    }
}
