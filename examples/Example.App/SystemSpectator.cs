using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using X.Spectator.Base;
using X.Spectator.Spectators;

namespace Example.App;

public class SystemSpectator : AutomatedSpectator<HealthStatus>
{
    public SystemSpectator(TimeSpan checkHealthPeriod, IStateEvaluator<HealthStatus> stateEvaluator, TimeSpan retentionPeriod, HealthStatus initialState)
        : base(checkHealthPeriod, retentionPeriod, stateEvaluator, initialState)
    {
        Console.WriteLine("SystemSpectator created");
    }
}