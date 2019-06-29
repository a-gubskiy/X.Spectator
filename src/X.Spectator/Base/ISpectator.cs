using System;

namespace X.Spectator.Base
{
    public interface ISpectator<TState> where TState : struct, IConvertible
    {
        event EventHandler<StateEventArgs<TState>> StateChanged;
        event EventHandler<HealthCheckEventArgs> HealthChecked;
        
        TState State { get; }
        
        void AddProbe(IProbe probe);

        void CheckHealth();
    }
}