using System;

namespace X.Monitor.Base
{
    public class StateEventArgs : EventArgs
    {
        public StateEventArgs(State state, DateTime timeStamp)
        {
            State = state;
            TimeStamp = timeStamp;
        }
        
        public DateTime TimeStamp { get; }
        public State State { get; }
    }
}