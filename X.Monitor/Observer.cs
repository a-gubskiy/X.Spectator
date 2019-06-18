using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using X.Monitor.Base;

namespace X.Monitor
{
    public class Observer : IObserver
    {
        public event EventHandler<StateEventArgs> StateChanged;
        public event EventHandler<EventArgs> HealthChecked;
        
        public State State { get;  private set; }
        
        public DateTime StateChangedDate { get;  private set; }
        
        public TimeSpan RetentionPeriod { get;  private set; }

        private readonly IList<IProbe> _probes;
        private readonly IStateChangingRule _stateChangingRule;
        private readonly List<(DateTime date, IDictionary<string, bool> results)> _stateRecords;

        public Observer(IStateChangingRule stateChangingRule, TimeSpan retentionPeriod)
        {
            RetentionPeriod = retentionPeriod;
            StateChangedDate = DateTime.UtcNow;

            _stateChangingRule = stateChangingRule;
            _probes = new List<IProbe>();
            _stateRecords = new List<(DateTime date, IDictionary<string, bool> results)>();
        }

        public void AddProbe(string name, Func<bool> func) => _probes.Add(new Probe(name, func));

        protected virtual void ChangeState(State state)
        {
            State = state;
            StateChangedDate = DateTime.UtcNow;
            
            StateChanged?.Invoke(this, new StateEventArgs(state, StateChangedDate));
        }

        public virtual void CheckHealth()
        {
            var results = new System.Collections.Concurrent.ConcurrentDictionary<string, bool>();

            Parallel.ForEach(_probes, probe =>
            {
                results.TryAdd(probe.Name, probe.GetState());
            });
            
            _stateRecords.Add((DateTime.UtcNow, results));
            
            //Recalculate state
            
            if (State == State.Normal)
            {
                if (_stateChangingRule.ShouldSwitchToDanger(StateChangedDate, _stateRecords))
                {
                    ChangeState(State.Danger);
                }
            }

            if (State == State.Danger)
            {
                if (_stateChangingRule.ShouldSwitchToNormal(StateChangedDate, _stateRecords))
                {
                    ChangeState(State.Normal);
                }
            }
            
            //cleanup state records
            _stateRecords.RemoveAll(o => o.date < DateTime.UtcNow.Add(RetentionPeriod));

            OnHealthChecked();
        }

        protected virtual void OnHealthChecked() => HealthChecked?.Invoke(this, EventArgs.Empty);
    }
    
    public class AutomatedObserver : Observer
    {
        public TimeSpan CheckHealthPeriod { get; }
        
        private readonly IScheduler _scheduler;

        public AutomatedObserver(
            TimeSpan checkHealthPeriod,
            TimeSpan retentionPeriod,
            IScheduler scheduler,
            IStateChangingRule stateChangingRule)
            : base(stateChangingRule, retentionPeriod)
        {
            CheckHealthPeriod = checkHealthPeriod;

            _scheduler = scheduler;
            _scheduler.Schedule(CheckHealthPeriod, CheckHealth);
        }
    }
}
 