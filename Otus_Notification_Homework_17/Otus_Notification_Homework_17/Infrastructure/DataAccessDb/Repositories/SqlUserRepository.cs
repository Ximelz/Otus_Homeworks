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
            await using (var dbConn = factory.CreateDataContext())
            {
                toDoUserModel = await dbConn.ToDoUsers
                                            .Where(x => x.UserId == userId)
                                            .FirstOrDefaultAsync(ct);

                if (toDoUserModel != null)
                    return ModelMapper.MapFromModel(toDoUserModel);
                return null;
            }
        }

        public async Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            ToDoUserModel? toDoUserModel;
            await using (var dbConn = factory.CreateDataContext())
            {
                toDoUserModel = await dbConn.ToDoUsers
                                            .Where(x => x.TelegramUserId == telegramUserId)
                                            .FirstOrDefaultAsync(ct);

                if (toDoUserModel != null)
                    return ModelMapper.MapFromModel(toDoUserModel);
                return null;
            }
        }

        public async Task Add(ToDoUser user, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            ToDoUserModel userModel = ModelMapper.MapToModel(user);

            await using (var dbConn = factory.CreateDataContext())
            {
                await dbConn.InsertAsync(userModel, token: ct);
            }
        }

        public async Task<IReadOnlyList<ToDoUser>> GetUsers(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            await using (var dbConn = factory.CreateDataContext())
            {
                var users = await dbConn.ToDoUsers.ToListAsync(ct);
                return users.MapListUsers();
            }
        }
    }
}
