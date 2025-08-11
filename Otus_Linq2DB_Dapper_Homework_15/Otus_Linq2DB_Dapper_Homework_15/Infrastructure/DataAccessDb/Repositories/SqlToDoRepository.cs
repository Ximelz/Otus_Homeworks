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

            await using (var dbConn = factory.CreateDataContext())
            {
                await dbConn.InsertAsync(ModelMapper.MapToModel(item), token: ct);
            }
        }

        public async Task Update(ToDoItem item, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            ToDoItemModel itemModel = ModelMapper.MapToModel(item);

            await using (var dbConn = factory.CreateDataContext())
            {
                await dbConn.UpdateAsync(itemModel, token: ct);
            }
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            await using (var dbConn = factory.CreateDataContext())
            {
                await dbConn.ToDoItems
                            .LoadWith(i => i.User)
                            .LoadWith(i => i.List)
                            .LoadWith(i => i.List!.User)
                            .Where(x => x.Id == id)
                            .DeleteAsync(ct);
            }
        }

        public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            await using (var dbConn = factory.CreateDataContext())
            {
                return await dbConn.ToDoItems
                                .LoadWith(i => i.User)
                                .LoadWith(i => i.List)
                                .LoadWith(i => i.List!.User)
                                .Where(x => x.UserId == userId)
                                .Where(y => y.Name == name)
                                .AnyAsync(ct);
            }
        }

        public async Task<int> CountActive(Guid userId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            
            await using (var dbConn = factory.CreateDataContext())
            {
                var items = await dbConn.ToDoItems
                                        .Where(x => x.UserId == userId)
                                        .Where(y => y.State == ToDoItemState.Active)
                                        .ToListAsync(ct);

                return items.Count();
            }
        }

        public async Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            await using (var dbConn = factory.CreateDataContext())
            {
                var items = await dbConn.ToDoItems
                                   .LoadWith(i => i.User)
                                   .LoadWith(i => i.List)
                                   .LoadWith(i => i.List!.User)
                                   .Where(x => x.UserId == userId)
                                   .ToListAsync(ct);

                return items.MapListItems()
                            .Where(predicate)
                            .ToList();
            }
        }

        public async Task<ToDoItem?> Get(Guid toDoItemId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            ToDoItemModel? modelItem;

            await using (var dbConn = factory.CreateDataContext())
            {
                modelItem = await dbConn.ToDoItems
                                        .LoadWith(i => i.User)
                                        .LoadWith(i => i.List)
                                        .LoadWith(i => i.List!.User)
                                        .Where(x => x.Id == toDoItemId)
                                        .FirstOrDefaultAsync(ct);

                if (modelItem != null)
                    return ModelMapper.MapFromModel(modelItem);
                return null;
            }
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            await using (var dbConn = factory.CreateDataContext())
            {
                var items = await dbConn.ToDoItems
                                   .LoadWith(i => i.User)
                                   .LoadWith(i => i.List)
                                   .LoadWith(i => i.List!.User)
                                   .Where(x => x.UserId == userId)
                                   .ToListAsync(ct);

                return items.MapListItems();
            }
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            await using (var dbConn = factory.CreateDataContext())
            {
                var items = await dbConn.ToDoItems
                                        .LoadWith(i => i.User)
                                        .LoadWith(i => i.List)
                                        .LoadWith(i => i.List!.User)
                                        .Where(x => x.UserId == userId)
                                        .ToListAsync(ct);

                return items.MapListItems()
                            .Where(y => y.State == ToDoItemState.Active)
                            .ToList();
            }
        }
    }
}
