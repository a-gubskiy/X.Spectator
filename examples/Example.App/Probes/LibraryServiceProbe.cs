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
            Status = HealthStatus.Unhealthy
        };

        try
        {
            if (_service.TotalBookCount > _minimumBookCount)
            {
                result.Status = HealthStatus.Healthy;
            }
        }
        catch (Exception ex)
        {
            result.Exception = ex;
            result.Data = new Dictionary<string, object>{{"exception-message", ex.Message}};
        }

        return Task.FromResult(result);
    }
}