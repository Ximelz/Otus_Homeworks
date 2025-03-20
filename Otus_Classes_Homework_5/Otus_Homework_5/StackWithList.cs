using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Homework_5
{
    public class StackWithList
    {
        public StackWithList(string[] args)
        {
            stackData = new List<string>(args);
        }
        private List<string> stackData;
        public int Size { get { return stackData.Count; } }
        public string Top
        {
            get
            {
                if (Size == 0)
                    return null;
                return stackData[Size - 1];
            }
        }

        public void Add(string value) => stackData.Add(value);

        public string Pop()
        {
            if (stackData.Count == 0)
                throw new Exception("Стек пустой");

            string result = stackData[Size - 1];
            stackData.RemoveAt(Size - 1);
            return result;
        }

        public static StackWithList Concat(params StackWithList[] stacks)
        {
            StackWithList result = new StackWithList(new string[0]);
            foreach (StackWithList stack in stacks)
                result.Merge(stack);
            return result;
        }
                
        /// <summary>
        /// Метод для вывода на консоль стека, без его разрушения. Был создан для тестирования.
        /// </summary>
        public void PrintStack()
        {
            Console.Write("Стек состоит из элементов: ");
            foreach(string stackItem in stackData)
                Console.Write($"\"{stackItem}\" ");
            Console.WriteLine("\r\n");
        }
    }

    public static class StackExtensionsWithList
    {
        public static void Merge(this StackWithList stack1, StackWithList stack2)
        {
            while (stack2.Size != 0)
                stack1.Add(stack2.Pop());
        }
    }
}
