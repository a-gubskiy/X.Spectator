using System;
using X.Spectator.Base;
using X.Spectator.Spectators;

namespace Example.API
{
    public class SystemSpectator : SpectatorBase<SystemState>
    {
        public SystemSpectator(IStateEvaluator<SystemState> stateEvaluator, TimeSpan retentionPeriod, SystemState initialState) 
            : base(stateEvaluator, retentionPeriod, initialState)
        {
        }
    }
}
