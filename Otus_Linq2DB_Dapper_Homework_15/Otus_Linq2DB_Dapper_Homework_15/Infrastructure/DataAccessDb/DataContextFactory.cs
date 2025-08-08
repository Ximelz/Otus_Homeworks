using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Linq2DB_Dapper_Homework_15
{
    public class DataContextFactory : IDataContextFactory<ToDoDataContext>
    {
        public DataContextFactory(string SqlConnStr)
        {
            SqlConnectionString = SqlConnStr;
        }
        private string SqlConnectionString;
        public ToDoDataContext CreateDataContext() => new ToDoDataContext(SqlConnectionString);
    }
}
