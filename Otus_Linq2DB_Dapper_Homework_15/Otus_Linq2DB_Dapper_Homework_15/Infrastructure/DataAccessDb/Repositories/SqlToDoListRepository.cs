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
            using (var dbConn = factory.CreateDataContext())
            {
                toDoListModel = dbConn.ListTable
                                      .LoadWith(i => i.User)
                                      .Where(x => x.Id == id)
                                      .FirstOrDefault();

                if (toDoListModel != null)
                    return ModelMapper.MapFromModel(toDoListModel);
                return null;
            }
        }

        public async Task<IReadOnlyList<ToDoList>> GetByUserId(Guid userId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            using (var dbConn = factory.CreateDataContext())
            {
                return dbConn.ListTable
                             .LoadWith(i => i.User)
                             .Where(x => x.UserId == userId)
                             .ToList()
                             .MapListLists();
            }
        }

        public async Task Add(ToDoList list, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            using (var dbConn = factory.CreateDataContext())
            {
                dbConn.Insert(ModelMapper.MapToModel(list));
            }
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            using (var dbConn = factory.CreateDataContext())
            {
                dbConn.ListTable.Where(x => x.Id == id).Delete();
            }
        }

        public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            using (var dbConn = factory.CreateDataContext())
            {
                return dbConn.ListTable
                             .Where(x => x.UserId == userId)
                             .Where(y => y.Name == name)
                             .Any();
            }
        }
    }
}
