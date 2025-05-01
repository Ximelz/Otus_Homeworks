using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 7. Добавление класса сервиса ToDoService
 *       7.1 Добавить интерфейс IToDoService
 *           
 *           public interface IToDoService
 *           {
 *              //Возвращает ToDoItem для UserId со статусом Active
 *              IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
 *              ToDoItem Add(User user, string name);
 *              void MarkCompleted(Guid id);
 *              void Delete(Guid id);
 *           }
 *           
 *       7.2 Создать класс ToDoService, который реализует интерфейс IToDoService. Перенести в него логику обработки команд
 *       7.3 Добавить использование IToDoService в UpdateHandler. Получать IToDoService нужно через конструктор
 *       7.4 Изменить формат обработки команды /addtask. Нужно сразу передавать имя задачи в команде. Пример: /addtask Новая задача
 *       7.5 Изменить формат обработки команды /removetask. Нужно сразу передавать номер задачи в команде. Пример: /removetask 2
 *
 * Лямбды. Добавление команды /find
 * Добавить метод IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate); в интерфейс IToDoRepository. Метод должен возвращать все задачи пользователя, которые удовлетворяют предикату.
 * Добавить метод IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix); в интерфейс IToDoService. Метод должен возвращать все задачи пользователя, которые начинаются на namePrefix. Для этого нужно использовать метод IToDoRepository.Find
 * Добавить обработку новой команды /find.
 * Пример команды: /find Имя
 * Вывод в консоль должен быть как в /showtask
 */

namespace Otus_Interfaces_Homework_6
{
    public interface IToDoService
    {
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);             //Метод получения всех активных задач пользователя.
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);                //Метод получения всех задач пользователя.
        ToDoItem Add(ToDoUser user, string name);                           //Метод добавления новой задачи.
        void MarkCompleted(Guid id, ToDoUser user);                         //Метод отметки задачи как выполненной.
        void Delete(Guid id);                                               //Метод удаления задачи.
        IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix);     //Метод поиска задач пользователя с указанным префиксом.
    }
}
