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
        public static IEnumerable<T>? GetBatchByNumber<T>(this IEnumerable<T>? inputEnumerable, int batchSize, int batchNumber)
        {
            if (inputEnumerable == null)
                throw new ArgumentNullException("Входной перечислитель не может быть null!");

            if (batchSize < 1)
                throw new ArgumentOutOfRangeException("Размер пачки не может быть меньше 1!");

            if (batchNumber < 0)
                throw new ArgumentOutOfRangeException("Номер пачти не может быть отридцательным!");

            int enumCount = inputEnumerable.Count();
            int totalBatchs = enumCount / batchSize;

            if (enumCount % batchSize != 0)
                totalBatchs++;

            if (totalBatchs <= batchNumber && batchNumber != 0)
                throw new ArgumentOutOfRangeException("Номер пачти превышает размер списка!");

            return inputEnumerable.Skip(batchSize * batchNumber).Take(batchSize);
        }
    }
}
