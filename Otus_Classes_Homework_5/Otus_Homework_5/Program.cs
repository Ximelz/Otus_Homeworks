using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

/* 
 * ДЗ №5
 * Описание/Пошаговая инструкция выполнения домашнего задания:
 * Стэк - тип данных, представляющий собой коллекцию элементов, организованную по принципу LIFO - Last In - First Out.
 * Данные в эту коллекцию могут добавляться только "сверху", и извлекать тоже сверху. Если мы добавили элемент1, а потом элемент2, то доступ к Элементу1 мы получим только после того как извлечем Элемент2.
 * 
 * В качестве примера стека может послужить стопка тарелок: мы кладем сверху тарелки, но если мы хотим взять тарелку из середины - надо для начала снять верхние.
 * 
 * Основное задание
 * Нужно создать класс Stack у которого будут следующие свойства:
 *  1. В нем будем хранить строки
 *  2. В качестве хранилища используйте список List
 *  3. Конструктор стека может принимать неограниченное количество входных параметров типа string, которые по порядку добавляются в стек
 *  4. Метод Add(string) - добавить элемент в стек
 *  5. Метод Pop() - извлекает верхний элемент и удаляет его из стека. При попытке вызова метода Pop у пустого стека - выбрасывать исключение с сообщением "Стек пустой"
 *  6. Свойство Size - количество элементов из Стека
 *  7. Свойство Top - значение верхнего элемента из стека. Если стек пустой - возвращать null
 * 
 * Доп. задание 1
 *  1. Создайте класс расширения StackExtensions и добавьте в него метод расширения Merge, который на вход принимает стек s1, и стек s2.
 *     Все элементы из s2 должны добавится в s1 в обратном порядке
 *     Сам метод должен быть доступен в класс Stack
 * 
 * Доп. задание 2
 *  1. В класс Stack и добавьте статический метод Concat, который на вход неограниченное количество параметров типа Stack
 *     и возвращает новый стек с элементами каждого стека в порядке параметров, но сами элементы записаны в обратном порядке
 * 
 * Доп. задание 3
 * Вместо коллекции - создать класс StackItem, который:
 *  1. Доступен только для класс Stack (отдельно объект класса StackItem вне Stack создать нельзя)
 *  2. Хранит текущее значение элемента стека
 *  3. Ссылку на предыдущий элемент в стеке
 *  4. Методы, описанные в основном задании переделаны под работу со StackItem
 */

namespace Otus_Homework_5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Stack with list.\r\n-----------------------------------\r\n");
                
                string[] argsStack1 = { "a", "b", "c" };
                StackWithList stack = new StackWithList(argsStack1);
                StackWithList mergeStack1 = new StackWithList(argsStack1);
                string[] argsStack2 = { "1", "2", "3" };
                StackWithList mergeStack2 = new StackWithList(argsStack2);
                Console.WriteLine("Исходный стек 1:");
                mergeStack1.PrintStack();
                Console.WriteLine("Исходный стек 2:");
                mergeStack2.PrintStack();
                mergeStack1.Merge(new StackWithList(argsStack2));
                Console.WriteLine("Стек после слияния методом Merge:");
                mergeStack1.PrintStack();

                string[] argsConcatStack1 = { "1", "2", "3" };
                string[] argsConcatStack2 = { "4", "5", "6", };
                string[] argsConcatStack3 = { "7", "8", "9" };
                string[] argsConcatStack4 = { "10", "11", "12" };
                string[] argsConcatStack5 = { "13", "14", "15" };

                StackWithList stack1 = new StackWithList(argsConcatStack1);
                StackWithList stack2 = new StackWithList(argsConcatStack2);
                StackWithList stack3 = new StackWithList(argsConcatStack3);
                StackWithList stack4 = new StackWithList(argsConcatStack4);
                StackWithList stack5 = new StackWithList(argsConcatStack5);

                Console.WriteLine("Исходный стек 1:");
                stack1.PrintStack();
                Console.WriteLine("Исходный стек 2:");
                stack2.PrintStack();
                Console.WriteLine("Исходный стек 3:");
                stack3.PrintStack();
                Console.WriteLine("Исходный стек 4:");
                stack4.PrintStack();
                Console.WriteLine("Исходный стек 5:");
                stack5.PrintStack();

                StackWithList concatStack = StackWithList.Concat(stack1, stack2, stack3, stack4, stack5);
                Console.WriteLine("Стек после слияния методом Concat:");
                concatStack.PrintStack();

                Console.WriteLine($"Вытаскиваем верхний элемент стека stack: {stack.Pop()}");
                stack.PrintStack();
                Console.WriteLine($"Вытаскиваем верхний элемент стека stack: {stack.Pop()}");
                stack.PrintStack();
                Console.WriteLine($"Верхний элемент в стеке stack: {stack.Top}");
                Console.WriteLine($"Вытаскиваем верхний элемент стека stack: {stack.Pop()}");
                stack.PrintStack();
                Console.WriteLine("Добавляем в стек stack элемент d");
                stack.Add("d");
                Console.WriteLine($"Верхний элемент в стеке stack: {stack.Top}");
                stack.PrintStack();
                Console.WriteLine($"Размер стека stack: {stack.Size}");
                stack.PrintStack();
                Console.WriteLine($"Вытаскиваем верхний элемент стека stack: {stack.Pop()}");
                stack.PrintStack();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                Console.WriteLine("\r\nStack without list.\r\n-----------------------------------\r\n");

                string[] argsStack1 = { "a", "b", "c" };
                StackWithoutList stack = new StackWithoutList(argsStack1);
                StackWithoutList mergeStack1 = new StackWithoutList(argsStack1);
                string[] argsStack2 = { "1", "2", "3" };
                StackWithoutList mergeStack2 = new StackWithoutList(argsStack2);
                Console.WriteLine("Исходный стек 1:");
                mergeStack1.PrintStack();
                Console.WriteLine("Исходный стек 2:");
                mergeStack2.PrintStack();
                mergeStack1.Merge(new StackWithoutList(argsStack2));
                Console.WriteLine("Стек после слияния методом Merge:");
                mergeStack1.PrintStack();

                string[] argsConcatStack1 = { "1", "2", "3" };
                string[] argsConcatStack2 = { "4", "5", "6", };
                string[] argsConcatStack3 = { "7", "8", "9" };
                string[] argsConcatStack4 = { "10", "11", "12" };
                string[] argsConcatStack5 = { "13", "14", "15" };

                StackWithoutList stack1 = new StackWithoutList(argsConcatStack1);
                StackWithoutList stack2 = new StackWithoutList(argsConcatStack2);
                StackWithoutList stack3 = new StackWithoutList(argsConcatStack3);
                StackWithoutList stack4 = new StackWithoutList(argsConcatStack4);
                StackWithoutList stack5 = new StackWithoutList(argsConcatStack5);

                Console.WriteLine("Исходный стек 1:");
                stack1.PrintStack();
                Console.WriteLine("Исходный стек 2:");
                stack2.PrintStack();
                Console.WriteLine("Исходный стек 3:");
                stack3.PrintStack();
                Console.WriteLine("Исходный стек 4:");
                stack4.PrintStack();
                Console.WriteLine("Исходный стек 5:");
                stack5.PrintStack();

                StackWithoutList concatStack = StackWithoutList.Concat(stack1, stack2, stack3, stack4, stack5);
                Console.WriteLine("Стек после слияния методом Concat:");
                concatStack.PrintStack();

                Console.WriteLine($"Вытаскиваем верхний элемент стека stack: {stack.Pop()}");
                stack.PrintStack();
                Console.WriteLine($"Вытаскиваем верхний элемент стека stack: {stack.Pop()}");
                stack.PrintStack();
                Console.WriteLine($"Верхний элемент в стеке stack: {stack.Top}");
                Console.WriteLine($"Вытаскиваем верхний элемент стека stack: {stack.Pop()}");
                stack.PrintStack();
                Console.WriteLine("Добавляем в стек stack элемент d");
                stack.Add("d");
                Console.WriteLine($"Верхний элемент в стеке stack: {stack.Top}");
                stack.PrintStack();
                Console.WriteLine($"Размер стека stack: {stack.Size}");
                stack.PrintStack();
                Console.WriteLine($"Вытаскиваем верхний элемент стека stack: {stack.Pop()}");
                stack.PrintStack();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
