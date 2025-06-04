using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_Scenario_Homework_11
{
    public class InMemoryScenarioContextRepository : IScenarioContextRepository
    {
        public InMemoryScenarioContextRepository()
        {
            scenarios = new Dictionary<long, ScenarioContext>();
        }
        private Dictionary<long, ScenarioContext> scenarios;
        public Task<ScenarioContext?> GetContext(long userId, CancellationToken ct)
        {
            if (scenarios.ContainsKey(userId))
                return Task.FromResult(scenarios[userId]);
            return Task.FromResult(new ScenarioContext(ScenarioType.None));
        }

        public Task SetContext(long userId, ScenarioContext context, CancellationToken ct)
        {
            if (!scenarios.ContainsKey(userId))
                scenarios.Add(userId, context);
            else
                scenarios[userId] = context;

            return Task.CompletedTask;
        }

        public Task ResetContext(long userId, CancellationToken ct)
        {
            if (scenarios.ContainsKey(userId))
                scenarios.Remove(userId);

            return Task.CompletedTask;
        }
    }
}
