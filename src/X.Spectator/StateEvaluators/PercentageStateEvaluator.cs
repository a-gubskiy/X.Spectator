using JetBrains.Annotations;
using X.Spectator.Base;

namespace X.Spectator.StateEvaluators;

/// <summary>
/// Percentage state evaluator.
/// </summary>
[PublicAPI]
public abstract class PercentageStateEvaluator : IStateEvaluator<double>
{
    public abstract double Evaluate(
        double currentState,
        DateTime stateChangedLastTime,
        IReadOnlyCollection<JournalRecord> journal);
}