using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace X.Spectator.Base;

/// <summary>
/// A spectator specialized for reporting and observing the application's <see cref="HealthStatus"/>.
/// </summary>
/// <remarks>
/// This interface is a specialization of <see cref="ISpectator{T}"/> where <typeparamref name="T"/> is
/// <see cref="HealthStatus"/>. Implementations are responsible for tracking, updating and exposing the
/// current health status of the system or a subsystem.
/// </remarks>
public interface IHealthStatusSpectator : ISpectator<HealthStatus>;