using JetBrains.Annotations;
using X.Spectator.Base;

namespace X.Spectator.StateEvaluators;

/// <summary>
/// Discrete state evaluator.
/// </summary>
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