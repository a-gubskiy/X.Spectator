using System.Collections.Immutable;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using X.Spectator.Base;

namespace X.Spectator.Spectators;

/// <summary>
/// The base class for all spectators.
/// </summary>
/// <typeparam name="TState"></typeparam>
[PublicAPI]
public class SpectatorBase<TState> : ISpectator<TState>
{
    private TState _state;

    private readonly ThreadSafeList<IProbe> _probes;
    private readonly ThreadSafeList<JournalRecord> _journal;
    private readonly IStateEvaluator<TState> _stateEvaluator;
    private readonly Stopwatch _stopwatch;

    public event EventHandler<StateEventArgs<TState>>? StateChanged;

    public event EventHandler<HealthCheckEventArgs>? HealthChecked;

    public virtual TState State => _state;

    public TimeSpan Uptime => _stopwatch.Elapsed;

    public string Name { get; set; }

    public IReadOnlyCollection<JournalRecord> Journal => _journal;

    public DateTime StateChangedDate { get; private set; }

    public TimeSpan RetentionPeriod { get; private set; }

    public SpectatorBase(IStateEvaluator<TState> stateEvaluator, TimeSpan retentionPeriod, TState initialState)
    {
        RetentionPeriod = retentionPeriod;
        StateChangedDate = DateTime.UtcNow;
        Name = "";

        _state = initialState;
        _stateEvaluator = stateEvaluator;
        _stopwatch = Stopwatch.StartNew();
        _probes = [];
        _journal = [];
    }

    public void AddProbe(IProbe probe) => _probes.Add(probe);

    protected virtual void ChangeState(TState state, IEnumerable<string> failedProbes)
    {
        _state = state;

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

        //cleanup state records
        _journal.RemoveAll(o => o.Time < now.Subtract(RetentionPeriod));
        
        _journal.Add(new JournalRecord(now, results));

        //Recalculate state
        var state = _stateEvaluator.Evaluate(State, StateChangedDate, _journal);

        if (!EqualityComparer<TState>.Default.Equals(State, state))
        {
            var failedProbes = results
                .Where(o => o.Value.Status == HealthStatus.Unhealthy)
                .Select(o => o.ProbeName)
                .ToImmutableList();

            ChangeState(state, failedProbes);
        }

        OnHealthChecked(now, results);
    }

    protected virtual void OnHealthChecked(DateTime now, IReadOnlyCollection<ProbeResult> results) =>
        HealthChecked?.Invoke(this, new HealthCheckEventArgs(now, results));
}