using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Example.App.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using X.Spectator.Base;

namespace Example.App.Probes;

/// <summary>
/// Example probe
/// </summary>
public class LibraryServiceProbe : IProbe
{
    private readonly LibraryService _service;
    private readonly int _minimumBookCount;

    public string Name => "Library Service Probe";

    public LibraryServiceProbe(LibraryService service, int minimumBookCount)
    {
        _service = service;
        _minimumBookCount = minimumBookCount;
    }

    public Task<ProbeResult> Check()
    {
        var result = new ProbeResult
        {
            Time = DateTime.UtcNow,
            ProbeName = Name,
            Value = HealthCheckResult.Degraded()
        };

        try
        {
            if (_service.TotalBookCount > _minimumBookCount)
            {
                result = result with { Value = HealthCheckResult.Healthy() };
            }
        }
        catch (Exception ex)
        {
            result = result with
            {
                Value = HealthCheckResult.Unhealthy(
                    exception: ex,
                    data: new Dictionary<string, object> { { "exception-message", ex.Message } }
                )
            };
        }

        return Task.FromResult(result);
    }
}