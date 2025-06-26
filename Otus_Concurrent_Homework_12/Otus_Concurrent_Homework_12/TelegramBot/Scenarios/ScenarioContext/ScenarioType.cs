using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Concurrent_Homework_12
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
