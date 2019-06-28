using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using X.Spectator.Base;

namespace X.Spectator
{
    public class Spectator<TState> : ISpectator<TState> where TState : struct
    {
        private TState _state;

        private readonly IList<IProbe> _probes;
        private readonly IStateEvaluator<TState> _stateEvaluator;
        private readonly List<JournalRecord> _journal;
        private readonly ReaderWriterLockSlim _journalLock = new ReaderWriterLockSlim();
        private readonly ReaderWriterLockSlim _stateLock = new ReaderWriterLockSlim();
        
        public event EventHandler<StateEventArgs<TState>> StateChanged;
        public event EventHandler<HealthCheckEventArgs> HealthChecked;

        public virtual TState State
        {
            get
            {
                _stateLock.EnterReadLock();
                
                try
                {
                    return _state;
                }
                finally
                {
                    _stateLock.ExitReadLock();
                }
            }
        }

        public IReadOnlyCollection<JournalRecord> Journal
        {
            get
            {
                _journalLock.EnterReadLock();
                
                try
                {
                    return _journal;
                }
                finally
                {
                    _journalLock.ExitReadLock();
                }
            }
        }
        
        public DateTime StateChangedDate { get; private set; }

        public TimeSpan RetentionPeriod { get; private set; }

        public Spectator(IStateEvaluator<TState> stateEvaluator, TimeSpan retentionPeriod)
        {
            RetentionPeriod = retentionPeriod;
            StateChangedDate = DateTime.UtcNow;

            _stateEvaluator = stateEvaluator;
            _probes = new List<IProbe>();
            _journal = new List<JournalRecord>();
        }

        public void AddProbe(IProbe probe) => _probes.Add(probe);

        protected virtual void ChangeState(TState state, IEnumerable<string> failedProbes)
        {
            _stateLock.EnterWriteLock();

            try
            {
                _state = state;
            }
            finally
            {
                _stateLock.ExitWriteLock();
            }

            StateChangedDate = DateTime.UtcNow;

            StateChanged?.Invoke(this, new StateEventArgs<TState>(state, StateChangedDate, failedProbes));
        }

        public virtual void CheckHealth()
        {
            var results = new Stack<Record>();

            var tasks = _probes
                .Select(async o => { results.Push(new Record {Name = o.Name, Value = await o.Ready()}); })
                .ToArray();

            Task.WaitAll(tasks);

            var now = DateTime.UtcNow;
            
            _journalLock.EnterWriteLock();
            
            try
            {
                //cleanup state records
                _journal.RemoveAll(o => o.Time < now.Subtract(RetentionPeriod));
                
                _journal.Add(new JournalRecord(now, results));
            }
            finally
            {
                _journalLock.ExitWriteLock();
            }

            //Recalculate state
            var state = _stateEvaluator.Evaluate(State, StateChangedDate, _journal);

            if (EqualityComparer<TState>.Default.Equals(State , state))
            {
                ChangeState(state, results.Where(o => !o.Value).Select(o => o.Name));
            }
         
            OnHealthChecked(now, results);
        }

        protected virtual void OnHealthChecked(DateTime now, IReadOnlyCollection<Record> results) =>
            HealthChecked?.Invoke(this, new HealthCheckEventArgs(now, results));
    }

    public class AutomatedSpectator<TState> : Spectator<TState> where TState : struct
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
 