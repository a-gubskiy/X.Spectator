using System;
using JetBrains.Annotations;

namespace X.Spectator.Base;

[PublicAPI]
public interface ISpectator<TState>
{
    /// <summary>
    /// Event that is triggered when the state of the spectator changes.
    /// </summary>
    event EventHandler<StateEventArgs<TState>> StateChanged;
    
    /// <summary>
    /// Event that is triggered when the health of the spectator is checked.
    /// </summary>
    event EventHandler<HealthCheckEventArgs> HealthChecked;
        
    /// <summary>
    /// State of the spectator.
    /// </summary>
    TState State { get; }
        
    /// <summary>
    /// Uptime of the spectator.
    /// </summary>
    TimeSpan Uptime { get; }
        
    /// <summary>
    /// Name of the spectator.
    /// </summary>
    string Name { get; }
     
    /// <summary>
    /// Adds a probe to the spectator.
    /// </summary>
    /// <param name="probe"></param>
    void AddProbe(IProbe probe);

    /// <summary>
    /// Checks the health of the spectator.
    /// </summary>
    void CheckHealth();
}