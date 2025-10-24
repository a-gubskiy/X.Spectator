using System.Collections.Immutable;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using X.Spectator.Base;

namespace X.Spectator.Spectators;

/// <summary>
/// The base class for all spectators.
/// Provides common plumbing for probe management, journaling of probe results,
/// state evaluation and notification when the observed state changes or a health check completes.
/// </summary>
/// <typeparam name="TState">The type representing the evaluated state (for example an enum or custom state object).</typeparam>
[PublicAPI]
public class SpectatorBase<TState> : ISpectator<TState>
{
    /// <summary>
    /// Backing field for the current state.
    /// </summary>
    private TState _state;

    /// <summary>
    /// Thread-safe collection of configured probes to execute during health checks.
    /// </summary>
    private readonly ThreadSafeList<IProbe> _probes;

    /// <summary>
    /// Thread-safe list of recorded probe results (journal).
    /// </summary>
    private readonly ThreadSafeList<JournalRecord> _journal;

    /// <summary>
    /// Evaluates the current state based on the previous state and the journal.
    /// </summary>
    private readonly IStateEvaluator<TState> _stateEvaluator;

    /// <summary>
    /// Stopwatch used to report uptime.
    /// </summary>
    private readonly Stopwatch _stopwatch;

    /// <summary>
    /// Raised when the spectator's state has changed.
    /// Subscribers receive the new state, the date/time of the change and any failed probe names.
    /// </summary>
    public event EventHandler<StateEventArgs<TState>>? StateChanged;

    /// <summary>
    /// Raised after a health check completes.
    /// Subscribers receive the timestamp for the check and the collection of probe results.
    /// </summary>
    public event EventHandler<HealthCheckEventArgs>? HealthChecked;

    /// <summary>
    /// Gets the current evaluated state.
    /// </summary>
    public virtual TState State => _state;

    /// <summary>
    /// Time elapsed since the spectator was created (uptime).
    /// </summary>
    public TimeSpan Uptime => _stopwatch.Elapsed;

    /// <summary>
    /// Optional human readable name for the spectator instance.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Read-only view of the journal of recorded probe results.
    /// </summary>
    public IReadOnlyCollection<JournalRecord> Journal => _journal;

    /// <summary>
    /// The UTC date/time when the state last changed.
    /// </summary>
    public DateTime StateChangedDate { get; private set; }

    /// <summary>
    /// How long to retain journal entries before they are pruned.
    /// </summary>
    public TimeSpan RetentionPeriod { get; private set; }

    /// <summary>
    /// Creates a new instance of <see cref="SpectatorBase{TState}"/>.
    /// </summary>
    /// <param name="stateEvaluator">Evaluator that determines the new state from the journal and previous state.</param>
    /// <param name="retentionPeriod">Duration to keep journal entries.</param>
    /// <param name="initialState">Initial state value.</param>
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

    /// <summary>
    /// Adds a probe to be executed during health checks.
    /// Probes are stored in a thread-safe collection.
    /// </summary>
    /// <param name="probe">Probe instance to add.</param>
    public void AddProbe(IProbe probe) => _probes.Add(probe);

    /// <summary>
    /// Changes the internal state, updates the <see cref="StateChangedDate"/>, and raises the <see cref="StateChanged"/> event.
    /// Protected so derived classes can override or extend behavior.
    /// </summary>
    /// <param name="state">The new state.</param>
    /// <param name="failedProbes">Names of probes that contributed to the state change (e.g. failed probes).</param>
    protected virtual void ChangeState(TState state, IEnumerable<string> failedProbes)
    {
        _state = state;

        StateChangedDate = DateTime.UtcNow;

        StateChanged?.Invoke(this, new StateEventArgs<TState>(state, StateChangedDate, failedProbes));
    }

    /// <summary>
    /// Executes all configured probes (concurrently), records results to the journal,
    /// prunes old records based on <see cref="RetentionPeriod"/>, evaluates the new state
    /// using the provided <see cref="IStateEvaluator{TState}"/> and raises <see cref="StateChanged"/>
    /// if the evaluated state differs from the current <see cref="State"/>.
    /// Finally, raises the <see cref="HealthChecked"/> event with the probe results.
    /// </summary>
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

    /// <summary>
    /// Invokes the <see cref="HealthChecked"/> event.  Protected virtual to allow overrides.
    /// </summary>
    /// <param name="now">Timestamp for when the health check completed.</param>
    /// <param name="results">Collection of probe results produced by this check.</param>
    protected virtual void OnHealthChecked(DateTime now, IReadOnlyCollection<ProbeResult> results) =>
        HealthChecked?.Invoke(this, new HealthCheckEventArgs(now, results));
}