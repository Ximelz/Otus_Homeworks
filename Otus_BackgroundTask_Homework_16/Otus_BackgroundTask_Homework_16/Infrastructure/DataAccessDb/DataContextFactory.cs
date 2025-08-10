using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_BackgroundTask_Homework_16
{
    public class DataContextFactory : IDataContextFactory<ToDoDataContext>
    {
        public DataContextFactory(string SqlConnStr)
        {
            SqlConnectionString = SqlConnStr;
        }
        private readonly string SqlConnectionString;
        public ToDoDataContext CreateDataContext() => new ToDoDataContext(SqlConnectionString);
    }
}
