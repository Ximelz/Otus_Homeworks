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
 */

namespace Otus_Interfaces_Homework_6
{
    public interface IToDoService
    {
        IReadOnlyList<ToDoItem> GetActiveByUserID(Guid userId);     //Метод получения всех активных задач пользователя.
        IReadOnlyList<ToDoItem> GetAllByUserID(Guid userId);        //Добавлено вне задания. Метод получения всех задач пользователя.
        ToDoItem Add(ConsoleUser user, string name);                //Метод добавления новой задачи.
        void MarkCompleted(Guid id);                                //Метод отметки задачи как выполненненой.
        void Delete(Guid id);                                       //Метод удаления задачи.
        public int GetTasksCount();                                 //Добавлено вне задания. Метод получения количества сохраненных задач.
    }
}
