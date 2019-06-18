using System;
using System.Threading;
using System.Threading.Tasks;
using X.Monitor.Base;

namespace X.Monitor
{
    public class SimpleScheduler : IScheduler
    {
        public void Schedule(TimeSpan interval, Action action)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            Task.Run(async () =>
                {
                    while (!cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        action();
                        await Task.Delay(interval, cancellationTokenSource.Token);
                    }
                }, cancellationTokenSource.Token
            );
        }
    }
}