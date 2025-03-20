using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Homework_5
{
    public class StackWithoutList
    {
        public StackWithoutList(string[] args)
        {
            foreach (string arg in args)
                Add(arg);
        }
        private StackItem topItem = null;
        public int Size
        {
            get
            {
                if (topItem == null)
                {
                    return 0;
                }
                return StackDescent(topItem);
            }
        }
        public string Top
        {
            get
            {
                if (topItem == null)
                    return null;
                return topItem.value;
            }
        }
        public void Add(string value)
        {
            if (topItem != null)
                topItem = new StackItem(value, topItem);
            else
                topItem = new StackItem(value);
        }
        public string Pop()
        {
            if (topItem == null)
                throw new Exception("Стек пустой");

            string result = Top;
            topItem = topItem.previousItem;
            return result;
        }

        public static StackWithoutList Concat(params StackWithoutList[] stacks)
        {
            StackWithoutList result = new StackWithoutList(new string[0]);
            foreach (StackWithoutList stack in stacks)
                result.Merge(stack);
            return result;
        }

        private int StackDescent(StackItem item)
        {
            int i;
            if (item.previousItem != null)
            {
                i = StackDescent(item.previousItem);
                i++;
                return i;
            }

            i = 1;
            return i;
        }

        private class StackItem
        {
            public StackItem(string value)
            {
                previousItem = null;
                this.value = value;
            }
            public StackItem(string value, StackItem item)
            {
                previousItem = item;
                this.value = value;
            }
            public string value { get; private set; }
            public StackItem previousItem { get; private set; }
        }

        /// <summary>
        /// Метод для вывода на консоль стека, без его разрушения. Был создан для тестирования.
        /// </summary>
        public void PrintStack()
        {
            Console.Write("Стек состоит из элементов: ");
            if (topItem != null)
            {
                StackItem item = topItem;
                Console.Write($"\"{item.value}\" ");
                while (true)
                {
                    item = item.previousItem;
                    if (item == null)
                        break;
                    Console.Write($"\"{item.value}\" ");
                }
            }
            Console.WriteLine("\r\n");
        }
    }

    public static class StackExtensionsWithOutList
    {
        public static void Merge(this StackWithoutList stack1, StackWithoutList stack2)
        {
            while (stack2.Size != 0)
                stack1.Add(stack2.Pop());
        }
    }
}
