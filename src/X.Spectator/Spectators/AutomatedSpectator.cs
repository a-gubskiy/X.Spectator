using System;
using X.Spectator.Base;

namespace X.Spectator.Spectators
{
    public interface IAutomatedSpectator<TState> : ISpectator<TState> where TState : struct
    {
        void Start();
    }

    public class AutomatedSpectator<TState> : SpectatorBase<TState>, IAutomatedSpectator<TState> where TState : struct
    {
        public TimeSpan CheckHealthPeriod { get; }

        private readonly System.Timers.Timer _timer;

        public AutomatedSpectator(
            TimeSpan checkHealthPeriod,
            TimeSpan retentionPeriod,
            IStateEvaluator<TState> stateEvaluator)
            : base(stateEvaluator, retentionPeriod)
        {
            CheckHealthPeriod = checkHealthPeriod;

            _timer = new System.Timers.Timer(CheckHealthPeriod.TotalMilliseconds);
            _timer.Elapsed += (sender, args) => CheckHealth();
            _timer.AutoReset = true;
        }

        public void Start() => _timer.Start();
    }
}