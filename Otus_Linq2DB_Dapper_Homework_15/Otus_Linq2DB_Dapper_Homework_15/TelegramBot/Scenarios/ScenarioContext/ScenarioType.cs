using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Linq2DB_Dapper_Homework_15
{
    /// <summary>
    /// Виды сценариев.
    /// </summary>
    public enum ScenarioType
    {
        None,
        AddTask,
        RemoveTask,
        CompleteTask,
        FindTask,
        AddList,
        DeleteList
    }
}
