using JetBrains.Annotations;

namespace X.Spectator.Base;

/// <summary>
/// Represents a spectator that observes and exposes a piece of state of type <typeparamref name="TState"/>.
/// </summary>
/// <remarks>
/// Implementations raise events when the spectator's state changes and when health checks are performed.
/// The interface also exposes metadata such as the spectator's name and uptime, and allows probes to be registered.
/// </remarks>
[PublicAPI]
public interface ISpectator<TState>
{
    /// <summary>
    /// Occurs when the spectator's state changes.
    /// </summary>
    /// <remarks>
    /// Subscribers receive a <see cref="StateEventArgs{TState}"/> containing the new state and related information.
    /// </remarks>
    event EventHandler<StateEventArgs<TState>> StateChanged;

    /// <summary>
    /// Occurs when the spectator's health is checked.
    /// </summary>
    /// <remarks>
    /// Subscribers receive a <see cref="HealthCheckEventArgs"/> containing the results of the health check.
    /// </remarks>
    event EventHandler<HealthCheckEventArgs> HealthChecked;

    /// <summary>
    /// Gets the current state observed by the spectator.
    /// </summary>
    TState State { get; }

    /// <summary>
    /// Gets the amount of time since the spectator started or was last (re)initialized.
    /// </summary>
    TimeSpan Uptime { get; }

    /// <summary>
    /// Gets the human-readable name of the spectator.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Registers a probe with the spectator.
    /// </summary>
    /// <param name="probe">The probe to add. The spectator will execute or include this probe when performing health checks.</param>
    void AddProbe(IProbe probe);

    /// <summary>
    /// Initiates a health check run for the spectator.
    /// </summary>
    /// <remarks>
    /// Implementations should execute registered probes, update health-related state, and raise the <see cref="HealthChecked"/> event.
    /// This method is expected to be non-blocking for callers that rely on events, but implementations may run checks synchronously.
    /// </remarks>
    void CheckHealth();
}