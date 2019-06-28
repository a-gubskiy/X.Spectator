using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace X.Spectator.Base
{
    public class StateEventArgs<TState> : EventArgs
    {
        public StateEventArgs(TState state, DateTime timeStamp, IEnumerable<string> failedProbes)
        {
            State = state;
            TimeStamp = timeStamp;
            FailedProbes = failedProbes.ToImmutableList();
        }
        
        public IReadOnlyCollection<string> FailedProbes { get; }
        
        public DateTime TimeStamp { get; }
        
        public TState State { get; }
    }
}