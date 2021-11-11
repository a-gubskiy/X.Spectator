using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using X.Spectator.Base;

namespace X.Spectator.StateEvaluators;

[PublicAPI]
public abstract class PercentageStateEvaluator : IStateEvaluator<double>
{
    public abstract double Evaluate(
        double currentState,
        DateTime stateChangedLastTime,
        IReadOnlyCollection<JournalRecord> journal);
}