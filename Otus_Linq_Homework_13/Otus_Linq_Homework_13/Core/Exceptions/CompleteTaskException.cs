using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Linq_Homework_13
{
    public class CompleteTaskException : Exception
    {
        /// <summary>
        /// Исключение при попытке отметить уже выполненную задачу выполненной.
        /// </summary>
        /// <param name="task">Id задачи.</param>
        public CompleteTaskException(Guid id) : base($"Задача \"{id}\" уже была отмечена выполненной!") { }
    }
}
