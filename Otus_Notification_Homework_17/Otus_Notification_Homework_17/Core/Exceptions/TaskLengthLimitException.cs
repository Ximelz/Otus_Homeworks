using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Notification_Homework_17
{
    class TaskLengthLimitException : Exception
    {
        /// <summary>
        /// Исключение при вводе слишком длинного описания задачи.
        /// </summary>
        /// <param name="taskLength">Длина описания текущей задачи.</param>
        /// <param name="taskLengthLimit">Максимальная длина описания задачи.</param>
        public TaskLengthLimitException(int taskLength, int taskLengthLimit) : base($"Длина задачи \"{taskLength}\" превышает максимально допустимое значение \"{taskLengthLimit}\"") { }
    }
}
