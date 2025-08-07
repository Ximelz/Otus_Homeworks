using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Otus_Linq2DB_Dapper_Homework_15
{
    public class SqlToDoRepository : IToDoRepository
    {
        public SqlToDoRepository(IDataContextFactory<ToDoDataContext> factory)
        {
            this.factory = factory;
        }
        private IDataContextFactory<ToDoDataContext> factory;

        public async Task Add(ToDoItem item, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            using (var dbConn = factory.CreateDataContext())
            {
                dbConn.Insert(ModelMapper.MapToModel(item));
            }
        }

        public async Task Update(ToDoItem item, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            ToDoItemModel itemModel = ModelMapper.MapToModel(item);

            using (var dbConn = factory.CreateDataContext())
            {
                dbConn.Update(itemModel);
            }
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            using (var dbConn = factory.CreateDataContext())
            {
                dbConn.ItemTable
                      .LoadWith(i => i.User)
                      .LoadWith(i => i.List)
                      .LoadWith(i => i.List!.User)
                      .Where(x => x.Id == id)
                      .Delete();
            }
        }

        public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            using (var dbConn = factory.CreateDataContext())
            {
                return dbConn.ItemTable
                             .LoadWith(i => i.User)
                             .LoadWith(i => i.List)
                             .LoadWith(i => i.List!.User)
                             .Where(x => x.UserId == userId)
                             .Where(y => y.Name == name)
                             .Any();
            }
        }

        public async Task<int> CountActive(Guid userId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            
            using (var dbConn = factory.CreateDataContext())
            {
                return dbConn.ItemTable.Where(x => x.UserId == userId).Where(y => y.State == ToDoItemState.Active).ToList().Count();
            }
        }

        public async Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            using (var dbConn = factory.CreateDataContext())
            {
                return dbConn.ItemTable
                             .LoadWith(i => i.User)
                             .LoadWith(i => i.List)
                             .LoadWith(i => i.List!.User)
                             .Where(x => x.UserId == userId)
                             .ToList()
                             .MapListItems()
                             .Where(predicate)
                             .ToList();
            }
        }

        public async Task<ToDoItem?> Get(Guid toDoItemId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            ToDoItemModel? modelItem;

            using (var dbConn = factory.CreateDataContext())
            {
                modelItem = dbConn.ItemTable
                                  .LoadWith(i => i.User)
                                  .LoadWith(i => i.List)
                                  .LoadWith(i => i.List!.User)
                                  .Where(x => x.Id == toDoItemId)
                                  .FirstOrDefault();

                if (modelItem != null)
                    return ModelMapper.MapFromModel(modelItem);
                return null;
            }
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            using (var dbConn = factory.CreateDataContext())
            {
                return dbConn.ItemTable
                             .LoadWith(i => i.User)
                             .LoadWith(i => i.List)
                             .LoadWith(i => i.List!.User)
                             .Where(x => x.UserId == userId)
                             .ToList()
                             .MapListItems();
            }
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            using (var dbConn = factory.CreateDataContext())
            {
                return dbConn.ItemTable
                             .LoadWith(i => i.User)
                             .LoadWith(i => i.List)
                             .LoadWith(i => i.List!.User)
                             .Where(x => x.UserId == userId)
                             .ToList()
                             .MapListItems()
                             .Where(y => y.State == ToDoItemState.Active)
                             .ToList();
            }
        }
    }
}
