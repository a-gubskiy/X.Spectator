using System.Collections.Immutable;
using JetBrains.Annotations;

namespace X.Spectator.Base;

/// <summary>
/// Event arguments produced when a health check run completes.
/// </summary>
/// <remarks>
/// Contains the timestamp when the check was performed and the collection of probe results.
/// The results are stored as an immutable list to prevent modification by consumers.
/// </remarks>
public class HealthCheckEventArgs : EventArgs
{
    /// <summary>
    /// Gets the UTC timestamp when the health check was executed.
    /// </summary>
    public DateTime TimeStamp { get; }

    /// <summary>
    /// Gets the collection of probe results produced by the health check.
    /// </summary>
    [NotNull]
    public IReadOnlyCollection<ProbeResult> Results { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HealthCheckEventArgs"/> class.
    /// </summary>
    /// <param name="timeStamp">The time when the health check run occurred.</param>
    /// <param name="results">The probe results produced by the health check. This collection will be copied into an immutable list.</param>
    public HealthCheckEventArgs(DateTime timeStamp, IReadOnlyCollection<ProbeResult> results)
    {
        TimeStamp = timeStamp;
        Results = results.ToImmutableList();
    }
}