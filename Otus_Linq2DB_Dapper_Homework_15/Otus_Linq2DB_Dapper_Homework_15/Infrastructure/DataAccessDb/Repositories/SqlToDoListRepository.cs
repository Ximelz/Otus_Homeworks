using LinqToDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Linq2DB_Dapper_Homework_15
{
    public class SqlToDoListRepository : IToDoListRepository
    {
        public SqlToDoListRepository(IDataContextFactory<ToDoDataContext> factory)
        {
            this.factory = factory;
        }
        private IDataContextFactory<ToDoDataContext> factory;

        public async Task<ToDoList?> Get(Guid id, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            ToDoListModel? toDoListModel;
            await using (var dbConn = factory.CreateDataContext())
            {
                toDoListModel = await dbConn.ToDoLists
                                            .LoadWith(i => i.User)
                                            .Where(x => x.Id == id)
                                            .FirstOrDefaultAsync();

                if (toDoListModel != null)
                    return ModelMapper.MapFromModel(toDoListModel);
                return null;
            }
        }

        public async Task<IReadOnlyList<ToDoList>> GetByUserId(Guid userId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            await using (var dbConn = factory.CreateDataContext())
            {
                var lists = await dbConn.ToDoLists
                                    .LoadWith(i => i.User)
                                    .Where(x => x.UserId == userId)
                                    .ToListAsync();

                return lists.MapListLists();
            }
        }

        public async Task Add(ToDoList list, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            await using (var dbConn = factory.CreateDataContext())
            {
                await dbConn.InsertAsync(ModelMapper.MapToModel(list));
            }
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            await using (var dbConn = factory.CreateDataContext())
            {
                await dbConn.ToDoLists.Where(x => x.Id == id).DeleteAsync();
            }
        }

        public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            await using (var dbConn = factory.CreateDataContext())
            {
                return await dbConn.ToDoLists
                             .Where(x => x.UserId == userId)
                             .Where(y => y.Name == name)
                             .AnyAsync();
            }
        }
    }
}
