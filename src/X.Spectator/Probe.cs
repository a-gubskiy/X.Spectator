using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using X.Spectator.Base;

namespace X.Spectator;

[PublicAPI]
public class Probe : IProbe
{
    private readonly Func<Task<ProbeResult>> _func;
        
    public string Name { get; }

    public Probe(string name, Func<Task<ProbeResult>> func)
    {
        Name = name;
        _func = func;
    }

    public async Task<ProbeResult> Check()
    {
        try
        {
            return await _func().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return new ProbeResult
            {
                ProbeName = Name,
                Time = DateTime.UtcNow,
                Status = HealthStatus.Unhealthy,
                Data = ImmutableDictionary<string, object>.Empty, 
                Exception = ex
            };
        }
    }
}