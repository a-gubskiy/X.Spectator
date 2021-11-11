using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using X.Spectator.Base;

namespace X.Spectator.StateEvaluators;

[PublicAPI]
public abstract class DiscreteStateEvaluator : IStateEvaluator<int>
{
    protected readonly int Min;
    protected readonly int Max;
    protected readonly int Step;

    public DiscreteStateEvaluator(int min, int max, int step)
    {
        Min = min;
        Max = max;
        Step = step;
    }

    public abstract int Evaluate(
        int currentState,
        DateTime stateChangedLastTime,
        IReadOnlyCollection<JournalRecord> journal);
}