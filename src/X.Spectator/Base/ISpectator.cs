using System;
using JetBrains.Annotations;

namespace X.Spectator.Base;

[PublicAPI]
public interface ISpectator<TState> where TState : struct, IConvertible
{
    event EventHandler<StateEventArgs<TState>> StateChanged;
    event EventHandler<HealthCheckEventArgs> HealthChecked;
        
    TState State { get; }
        
    TimeSpan Uptime { get; }
        
    string Name { get; }
        
    void AddProbe(IProbe probe);

    void CheckHealth();
}