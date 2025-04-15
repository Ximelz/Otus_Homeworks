using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otus_Interfaces_Homework_6;

/*
 * Добавление репозитория IUserRepository
 * Добавить интерфейс IUserRepository
 * interface IUserRepository
 * {
 *     ToDoUser? GetUser(Guid userId);
 *     ToDoUser? GetUserByTelegramUserId(long telegramUserId);
 *     void Add(ToDoUser user);
 * }
 * Создать класс InMemoryUserRepository, который реализует интерфейс IUserRepository. В качестве хранилища использовать List
 * Добавить использование IUserRepository в UserService. Получать IUserRepository нужно через конструктор
 */

namespace Otus_Annonumous_types_Tuple_Homework_7
{
    public interface IUserRepository
    {
        ToDoUser? GetUser(Guid userId);                             //Метод получения пользователя по guid id.
        ToDoUser? GetUserByTelegramUserId(long telegramUserId);     //Метод получения пользователя по telegram id/
        void Add(ToDoUser user);                                    //Метод добавления пользователя.
    }
}
