using System;
using System.Collections.Generic;
using X.Spectator.Base;

namespace X.Spectator.StateEvaluators
{
    public abstract class PercentageStateEvaluator : IStateEvaluator<double>
    {
        public abstract double Evaluate(
            double currentState,
            DateTime stateChangedLastTime,
            IReadOnlyCollection<JournalRecord> journal);
    }
}