using JetBrains.Annotations;

namespace X.Spectator.Base;

[PublicAPI]
public interface IStateEvaluator<TState>
{
    /// <summary>
    /// Evaluates the state of the spectator.
    /// </summary>
    /// <param name="currentState">
    /// Current state of the spectator.    
    /// </param>
    /// <param name="stateChangedLastTime">
    /// The time when the state of the spectator was changed last time.
    /// </param>
    /// <param name="journal">
    /// Journal of the spectator.
    /// </param>
    /// <returns></returns>
    TState Evaluate(TState currentState, DateTime stateChangedLastTime, IReadOnlyCollection<JournalRecord> journal);
}