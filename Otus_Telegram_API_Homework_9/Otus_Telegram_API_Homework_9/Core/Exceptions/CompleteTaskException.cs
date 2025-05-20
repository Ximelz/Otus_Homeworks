using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Telegram_API_Homework_9.Core
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
