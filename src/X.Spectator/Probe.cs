using JetBrains.Annotations;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using X.Spectator.Base;

namespace X.Spectator;

/// <summary>
/// Probe implementation.
/// </summary>
[PublicAPI]
public class Probe : IProbe
{
    /// <summary>
    /// Asynchronous delegate that executes the probe logic and returns a <see cref="ProbeResult"/>.
    /// </summary>
    private readonly Func<Task<ProbeResult>> _func;

    /// <summary>
    /// Gets the name of the probe.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Probe"/> class.
    /// </summary>
    /// <param name="name">The unique name of the probe. Used to identify the probe in results and logs.</param>
    /// <param name="func">An asynchronous function that performs the probe check and returns a <see cref="ProbeResult"/>.</param>
    public Probe(string name, Func<Task<ProbeResult>> func)
    {
        Name = name;

        _func = func;
    }

    /// <summary>
    /// Executes the probe logic and returns the result.
    /// </summary>
    /// <remarks>
    /// The provided probe function is awaited. If it completes successfully its <see cref="ProbeResult"/>
    /// is returned. If it throws an exception the exception is captured and an unhealthy <see cref="ProbeResult"/>
    /// is returned with the exception attached to the underlying <see cref="HealthCheckResult"/>.
    /// </remarks>
    /// <returns>A <see cref="Task{ProbeResult}"/> representing the asynchronous operation. The task result contains the probe outcome.</returns>
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
                Value = HealthCheckResult.Unhealthy(exception: ex)
            };
        }
    }
}