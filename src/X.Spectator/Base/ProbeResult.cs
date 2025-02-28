using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace X.Spectator.Base;

[PublicAPI]
public struct ProbeResult
{
    /// <summary>
    /// Probe name.
    /// </summary>
    public string ProbeName { get; set; }

    /// <summary>
    /// Probe execution time.
    /// </summary>
    public DateTime Time { get; set; }

    /// <summary>
    /// Result of the probe.
    /// </summary>
    public HealthCheckResult Value { get; set; }

    /// <summary>
    /// Returns the fully qualified type name of this instance.
    /// </summary>
    /// <returns>
    /// The fully qualified type name.
    /// </returns>
    public override string ToString() => $"{Time}: {Value}";
}