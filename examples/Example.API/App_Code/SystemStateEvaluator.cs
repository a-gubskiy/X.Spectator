using System;
using System.Collections.Generic;
using X.Spectator.Base;

namespace Example.API
{
    public class SystemStateEvaluator : IStateEvaluator<SystemState>
    {
        public SystemState Evaluate(
            SystemState currentState, 
            DateTime stateChangedLastTime, 
            IReadOnlyCollection<JournalRecord> journal)
        {
            throw new NotImplementedException();
        }
    }
}
