using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Otus_Notification_Homework_17
{
    public class SqlUserRepository : IUserRepository
    {
        public SqlUserRepository(IDataContextFactory<ToDoDataContext> factory)
        {
            this.factory = factory;
        }
        private IDataContextFactory<ToDoDataContext> factory;
        public async Task<ToDoUser?> GetUser(Guid userId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            ToDoUserModel? toDoUserModel;
            using (var dbConn = factory.CreateDataContext())
            {
                toDoUserModel = dbConn.UserTable.Where(x => x.UserId == userId).FirstOrDefault();
                if (toDoUserModel != null)
                    return ModelMapper.MapFromModel(toDoUserModel);
                return null;
            }
        }

        public async Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            ToDoUserModel? toDoUserModel;
            using (var dbConn = factory.CreateDataContext())
            {
                toDoUserModel = dbConn.UserTable.Where(x => x.TelegramUserId == telegramUserId).FirstOrDefault();
                if (toDoUserModel != null)
                    return ModelMapper.MapFromModel(toDoUserModel);
                return null;
            }
        }

        public async Task Add(ToDoUser user, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            ToDoUserModel userModel = ModelMapper.MapToModel(user);

            using (var dbConn = factory.CreateDataContext())
            {
                dbConn.Insert(userModel);
            }
        }

        public async Task<IReadOnlyList<ToDoUser>> GetUsers(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            using (var dbConn = factory.CreateDataContext())
            {
                return dbConn.UserTable.ToList().MapListUsers().ToList();
            }
        }
    }
}
