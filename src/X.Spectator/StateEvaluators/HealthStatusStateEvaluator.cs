using Microsoft.Extensions.Diagnostics.HealthChecks;
using X.Spectator.Base;

namespace X.Spectator.StateEvaluators;

/// <summary>
/// Abstract base class that evaluates health status based on current state and journal information.
/// Implements <see cref="IStateEvaluator{HealthStatus}"/> interface for health status evaluation.
/// </summary>
/// <remarks>
/// Concrete implementations of this class should define specific logic for evaluating health status
/// transitions based on the current state, time since last state change, and journal records.
/// </remarks>
public abstract class HealthStatusStateEvaluator : IStateEvaluator<HealthStatus>
{
    /// <summary>
    /// Evaluates the health status based on the current state, time since last state change, and journal records.
    /// </summary>
    /// <param name="currentState">The current health status that is being evaluated.</param>
    /// <param name="stateChangedLastTime">The timestamp when the current state was last changed.</param>
    /// <param name="journal">A collection of journal records that may influence the health status evaluation.</param>
    /// <returns>The evaluated <see cref="HealthStatus"/> based on the provided inputs.</returns>
    public abstract HealthStatus Evaluate(
        HealthStatus currentState, 
        DateTime stateChangedLastTime,
        IReadOnlyCollection<JournalRecord> journal);
}