using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Scenario_Homework_11
{
    /// <summary>
    /// Информация о сценарии.
    /// </summary>
    public class ScenarioContext
    {
        public ScenarioContext(ScenarioType scenario)
        {
            CurrentScenario = scenario;
            Data = new Dictionary<string, object>();
        }

        /// <summary>
        /// Id telegram пользователя.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Текущий вид сценария.
        /// </summary>
        public ScenarioType CurrentScenario { get; private set; }

        /// <summary>
        /// Текущий шаг в сценарии.
        /// </summary>
        public string? CurrentStep { get; set; }

        /// <summary>
        /// Данные, необходимые для выполнения сценария.
        /// </summary>
        public Dictionary<string, object> Data { get; private set; }
    }
}
