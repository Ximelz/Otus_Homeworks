using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otus_BackgroundTask_Homework_16
{
    public class InMemoryScenarioContextRepository : IScenarioContextRepository
    {
        private readonly ConcurrentDictionary<long, ScenarioContext> scenarios;
        public InMemoryScenarioContextRepository()
        {
            scenarios = new ConcurrentDictionary<long, ScenarioContext>();
        }
        public async Task<ScenarioContext?> GetContext(long userId, CancellationToken ct)
        {
            if (scenarios.ContainsKey(userId))
                return scenarios[userId];
            return null;
        }

        public Task SetContext(long userId, ScenarioContext context, CancellationToken ct)
        {
            if (!scenarios.ContainsKey(userId))
                scenarios.TryAdd(userId, context);
            else
                scenarios[userId] = context;

            return Task.CompletedTask;
        }

        public Task ResetContext(long userId, CancellationToken ct)
        {
            if (scenarios.ContainsKey(userId))
                scenarios.TryRemove(userId, out ScenarioContext? context);

            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<ScenarioContext>> GetContexts(CancellationToken ct)
        {
            return scenarios.Values.ToList();
        }
    }
}
