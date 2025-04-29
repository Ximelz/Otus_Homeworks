using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Annonumous_types_Tuple_Homework_7.Core.Exceptions
{
    class TaskCountLimitException : Exception
    {
        /// <summary>
        /// Исключение при добавлении задачи, выходящей за пределы максимально допустимого количества задач.
        /// </summary>
        /// <param name="taskCountLimit">Максимальное количество задач.</param>
        public TaskCountLimitException(int taskCountLimit) : base($"Превышено максимальное количество задач равное \"{taskCountLimit}\"") { }
    }
}
