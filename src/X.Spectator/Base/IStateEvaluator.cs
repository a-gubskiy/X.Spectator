using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace X.Spectator.Base
{
    [PublicAPI]
    public interface IStateEvaluator<TState>
    {
        TState Evaluate(
            TState currentState, 
            DateTime stateChangedLastTime, 
            IReadOnlyCollection<JournalRecord> journal);
    }
}