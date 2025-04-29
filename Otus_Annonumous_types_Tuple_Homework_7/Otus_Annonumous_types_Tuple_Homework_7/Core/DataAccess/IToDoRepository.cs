using Otus_Interfaces_Homework_6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Добавление репозитория IToDoRepository
 * Добавить интерфейс IToDoRepository
 * interface IToDoRepository
 * {
 *     IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);
 *     //Возвращает ToDoItem для UserId со статусом Active
 *     IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
 *     void Add(ToDoItem item);
 *     void Update(ToDoItem item);
 *     void Delete(Guid id);
 *     //Проверяет есть ли задача с таким именем у пользователя
 *     bool ExistsByName(Guid userId, string name);
 *     //Возвращает количество активных задач у пользователя
 *     int CountActive(Guid userId); 
 * }
 * Создать класс InMemoryToDoRepository, который реализует интерфейс IToDoRepository. В качестве хранилища использовать List
 * Добавить использование IToDoRepository в ToDoService. Получать IToDoRepository нужно через конструктор
 *
 * Лямбды. Добавление команды /find
 * Добавить метод IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate); в интерфейс IToDoRepository. Метод должен возвращать все задачи пользователя, которые удовлетворяют предикату.
 * Добавить метод IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix); в интерфейс IToDoService. Метод должен возвращать все задачи пользователя, которые начинаются на namePrefix. Для этого нужно использовать метод IToDoRepository.Find
 * Добавить обработку новой команды /find.
 * Пример команды: /find Имя
 * Вывод в консоль должен быть как в /showtask
 */

namespace Otus_Annonumous_types_Tuple_Homework_7
{
    public interface IToDoRepository
    {
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);                            //Метод получения списка всех задач пользователя.
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);                         //Метод получения списка активных задач пользователя.
        void Add(ToDoItem item);                                                        //Метод добавления задачи.
        void Update(ToDoItem item);                                                     //Метод обновления задачи.
        void Delete(Guid id);                                                           //Метод удаления задачи.
        bool ExistsByName(Guid userId, string name);                                    //Метод проверки наличия у пользователя задачи.
        int CountActive(Guid userId);                                                   //Метод получения количества задач пользователя.
        IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate);      //Метод для поиска задач по указанному предикату.
    }
}
