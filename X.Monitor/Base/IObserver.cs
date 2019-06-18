using System;

namespace X.Monitor.Base
{
    public interface IObserver
    {
        event EventHandler<StateEventArgs> StateChanged;
        event EventHandler<EventArgs> HealthChecked;
        
        State State { get; }
        
        void AddProbe(string name, Func<bool> func);
    }
}