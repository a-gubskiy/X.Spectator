using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;

namespace X.Spectator.Base
{
    [PublicAPI]
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