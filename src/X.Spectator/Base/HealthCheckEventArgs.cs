using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;

namespace X.Spectator.Base;

public class HealthCheckEventArgs : EventArgs
{
    public DateTime TimeStamp { get; }

    /// <summary>
    /// Health check results
    /// </summary>
    [NotNull]
    public IReadOnlyCollection<ProbeResult> Results { get; }

    public HealthCheckEventArgs(DateTime timeStamp, IReadOnlyCollection<ProbeResult> results)
    {
        TimeStamp = timeStamp;
        Results = results ?? ImmutableList<ProbeResult>.Empty;
    }
}