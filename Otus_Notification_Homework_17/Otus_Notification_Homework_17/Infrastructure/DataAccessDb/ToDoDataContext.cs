using LinqToDB;
using LinqToDB.Data;
using Otus_Notification_Homework_17;
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
        public ITable<ToDoUserModel> UserTable => this.GetTable<ToDoUserModel>();

        /// <summary>
        /// Выгруженная таблица списков задач.
        /// </summary>
        public ITable<ToDoListModel> ListTable => this.GetTable<ToDoListModel>();

        /// <summary>
        /// Выгруженная таблица задач.
        /// </summary>
        public ITable<ToDoItemModel> ItemTable => this.GetTable<ToDoItemModel>();


        /// <summary>
        /// Выгруженная таблица нотификаций.
        /// </summary>
        public ITable<NotificationModel> NotificationTable => this.GetTable<NotificationModel>();
    }
}
