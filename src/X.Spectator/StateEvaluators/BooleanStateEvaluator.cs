using JetBrains.Annotations;
using X.Spectator.Base;

namespace X.Spectator.StateEvaluators;

/// <summary>
/// Boolean state evaluator.
/// </summary>
[PublicAPI]
public abstract class BooleanStateEvaluator : IStateEvaluator<bool>
{
    public abstract bool Evaluate(
        bool currentState, 
        DateTime stateChangedLastTime,
        IReadOnlyCollection<JournalRecord> journal);
}