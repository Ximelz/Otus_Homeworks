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
        /// ����� ���������� LINQ ��� ���������.
        /// </summary>
        /// <param name="inputEnumerable">������ ������ �����.</param>
        /// <param name="batchSize">������ �����.</param>
        /// <param name="batchNumber">����� ������������ �����, ��������� � 0.</param>
        /// <returns>������ ����� �� ������� �����.</returns>
        /// <exception cref="InvalidOperationException">������, ���� ����� ������.</exception>
        public static List<T> GetBatchByNumber<T>(this IEnumerable<T>? inputEnumerable, int batchSize, int batchNumber)
        {
            var inputList = inputEnumerable.ToList();
            List<T> resultList = new List<T>();

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
