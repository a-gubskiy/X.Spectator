using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using X.Spectator.Base;

namespace Example.App;

public class SystemStateEvaluator : IStateEvaluator<SystemState>
{
    public SystemState Evaluate(SystemState currentState, DateTime stateChangedLastTime, IReadOnlyCollection<JournalRecord> journal)
    {
        var last = journal.Cast<JournalRecord?>().LastOrDefault();

        if (last == null)
        {
            return SystemState.Normal;
        }

        return last.Value.Values.Any(o => o.Status == HealthStatus.Unhealthy)
            ? SystemState.Danger
            : SystemState.Normal;
    }
}