using JetBrains.Annotations;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace X.Spectator.Base;

[PublicAPI]
public struct ProbeResult
{
    /// <summary>
    /// Name of the probe for which the result is generated.
    /// </summary>
    public string ProbeName { get; init; }

    /// <summary>
    /// Probe execution time.
    /// </summary>
    public DateTime Time { get; init; }

    /// <summary>
    /// Result of the probe.
    /// </summary>
    public HealthCheckResult Value { get; init; }

    /// <summary>
    /// Returns the fully qualified type name of this instance.
    /// </summary>
    /// <returns>
    /// The fully qualified type name.
    /// </returns>
    public override string ToString() => $"{Time}: {Value}";
}