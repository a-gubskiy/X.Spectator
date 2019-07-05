using System;
using System.Collections.Generic;
using X.Spectator.Base;

namespace X.Spectator.StateEvaluators
{
    public enum State
    {
        Normal, 
        Warning, 
        Danger,
        Down
    }

    public abstract class EnumerableStateEvaluators : IStateEvaluator<State>
    {
        public abstract State Evaluate(
            State currentState,
            DateTime stateChangedLastTime,
            IReadOnlyCollection<JournalRecord> journal);
    }
}