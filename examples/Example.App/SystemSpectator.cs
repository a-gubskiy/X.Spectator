using System;
using X.Spectator.Base;
using X.Spectator.Spectators;

namespace Example.App;

public class SystemSpectator : AutomatedSpectator<SystemState>
{
    public SystemSpectator(TimeSpan checkHealthPeriod, IStateEvaluator<SystemState> stateEvaluator, TimeSpan retentionPeriod, SystemState initialState)
        : base(checkHealthPeriod, retentionPeriod, stateEvaluator, initialState)
    {
        Console.WriteLine("SystemSpectator created");
    }
}