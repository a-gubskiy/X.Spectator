using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using X.Spectator.Base;

namespace X.Spectator.Spectators;

[PublicAPI]
public class SpectatorBase<TState> : ISpectator<TState> 
    where TState : struct, IConvertible
{
    private TState _state;

    private readonly IList<IProbe> _probes;
    private readonly IStateEvaluator<TState> _stateEvaluator;
    private readonly List<JournalRecord> _journal;
    private readonly ReaderWriterLockSlim _journalLock;
    private readonly ReaderWriterLockSlim _stateLock;
    private readonly Stopwatch _stopwatch;

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

    public TimeSpan Uptime => _stopwatch.Elapsed;

    public string Name { get; set; }

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

    public SpectatorBase(IStateEvaluator<TState> stateEvaluator, TimeSpan retentionPeriod, TState initialState)
    {
        RetentionPeriod = retentionPeriod;
        _state = initialState;
        StateChangedDate = DateTime.UtcNow;
            
        _stateEvaluator = stateEvaluator;
        _stopwatch = Stopwatch.StartNew();
        _probes = new List<IProbe>();
        _journal = new List<JournalRecord>();
        _journalLock = new ReaderWriterLockSlim();
        _stateLock = new ReaderWriterLockSlim();
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
        var results = new Stack<ProbeResult>();

        var tasks = _probes
            .Select(async o => { results.Push(await o.Check().ConfigureAwait(false)); })
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

        if (!EqualityComparer<TState>.Default.Equals(State, state))
        {
            ChangeState(state, results.Where(o => o.Status == HealthStatus.Unhealthy).Select(o => o.ProbeName));
        }

        OnHealthChecked(now, results);
    }

    protected virtual void OnHealthChecked(DateTime now, IReadOnlyCollection<ProbeResult> results) =>
        HealthChecked?.Invoke(this, new HealthCheckEventArgs(now, results));
}