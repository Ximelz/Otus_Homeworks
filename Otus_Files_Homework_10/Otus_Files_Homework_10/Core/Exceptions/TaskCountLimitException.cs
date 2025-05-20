using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Files_Homework_10
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
