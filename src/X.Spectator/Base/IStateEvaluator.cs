using System;
using System.Collections.Generic;

namespace X.Spectator.Base
{
    public interface IStateEvaluator<TState>
    {
        TState Evaluate(
            TState currentState, 
            DateTime stateChangedLastTime, 
            IReadOnlyCollection<JournalRecord> journal);
    }
    
}