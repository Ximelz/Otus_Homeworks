using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Concurrent_Homework_12
{
    class DuplicateTaskException : Exception
    {
        /// <summary>
        /// Исключение при вводе дубля задачи.
        /// </summary>
        /// <param name="task">Введенная задача.</param>
        public DuplicateTaskException(string task) : base($"Задача \"{task}\" уже существует!") { }
    }
}
