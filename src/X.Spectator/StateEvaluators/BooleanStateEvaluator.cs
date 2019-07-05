using System;
using System.Collections.Generic;
using X.Spectator.Base;

namespace X.Spectator.StateEvaluators
{
    public abstract class BooleanStateEvaluators : IStateEvaluator<bool>
    {
        public abstract bool Evaluate(
            bool currentState, 
            DateTime stateChangedLastTime,
            IReadOnlyCollection<JournalRecord> journal);
    }
}