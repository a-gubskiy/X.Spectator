using JetBrains.Annotations;

namespace X.Spectator.Base;

[PublicAPI]
public interface IStateEvaluator<TState>
{
    /// <summary>
    /// Evaluates the state of the spectator based on the current state, the time the state last changed, and the spectator's journal.
    /// </summary>
    /// <param name="currentState">
    /// Current state of the spectator.
    /// </param>
    /// <param name="stateChangedLastTime">
    /// The time when the state of the spectator was changed last time.
    /// </param>
    /// <param name="journal">
    /// Journal of the spectator containing historical records or events relevant to state evaluation.
    /// </param>
    /// <returns>
    /// The evaluated state of type <typeparamref name="TState"/>. Implementations should return the new or updated state resulting from the evaluation.
    /// </returns>
    TState Evaluate(TState currentState, DateTime stateChangedLastTime, IReadOnlyCollection<JournalRecord> journal);
}