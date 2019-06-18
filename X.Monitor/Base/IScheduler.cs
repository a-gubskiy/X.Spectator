using System;

namespace X.Monitor.Base
{
    public interface IScheduler
    {
        void Schedule(TimeSpan interval, Action action);
    }
}