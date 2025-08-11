using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Notification_Homework_17
{
    /// <summary>
    /// Класс для подключения и выгрузки данных из таблиц БД.
    /// </summary>
    public class ToDoDataContext : DataConnection
    {
        public ToDoDataContext(string connectionString) : base(ProviderName.PostgreSQL, connectionString) { }

        /// <summary>
        /// Выгруженная таблица пользователей.
        /// </summary>
        public ITable<ToDoUserModel> ToDoUsers => this.GetTable<ToDoUserModel>();

        /// <summary>
        /// Выгруженная таблица списков задач.
        /// </summary>
        public ITable<ToDoListModel> ToDoLists => this.GetTable<ToDoListModel>();

        /// <summary>
        /// Выгруженная таблица задач.
        /// </summary>
        public ITable<ToDoItemModel> ToDoItems => this.GetTable<ToDoItemModel>();
        /// <summary>
        /// Выгруженная таблица задач.
        /// </summary>
        public ITable<NotificationModel> Notifications => this.GetTable<NotificationModel>();
    }
}
