using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Annonumous_types_Tuple_Homework_7.Core.Exceptions
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
