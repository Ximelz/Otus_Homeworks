using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Linq_Homework_13
{
    public static class EnumerableExtension
    {
        /// <summary>
        /// Метод расширения LINQ для пагинации.
        /// </summary>
        /// <param name="inputEnumerable">Полный список задач.</param>
        /// <param name="batchSize">Размер пачки.</param>
        /// <param name="batchNumber">Номер возвращаемой пачки, нумерация с 0.</param>
        /// <returns>Список задач из искомой пачки.</returns>
        /// <exception cref="InvalidOperationException">Ошибка, если спиок пустой.</exception>
        public static List<T> GetBatchByNumber<T>(this IEnumerable<T>? inputEnumerable, int batchSize, int batchNumber)
        {
            List<T> resultList = new List<T>();

            if (inputEnumerable == null)
                return resultList;

            var inputList = inputEnumerable.ToList();

            int index = batchSize * batchNumber;
            int lastIndex = index + batchSize;

            if (lastIndex > inputList.Count - 1)
                lastIndex = inputList.Count;

            for (; index < lastIndex; index++)
                resultList.Add(inputList[index]);

            return resultList;
        }
    }
}
