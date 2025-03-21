using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using X.Spectator.Base;
using X.Spectator.StateEvaluators;

namespace Example.App;

public class SystemStateEvaluator : HealthStatusStateEvaluator
{
    public override HealthStatus Evaluate(HealthStatus currentState, DateTime stateChangedLastTime, IReadOnlyCollection<JournalRecord> journal)
    {
        var last = journal.LastOrDefault();

        if (last == null)
        {
            return HealthStatus.Healthy;
        }
        
        return last.Values.Any(o => o.Value.Status == HealthStatus.Unhealthy)
            ? HealthStatus.Degraded
            : HealthStatus.Healthy;
    }
}