using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using X.Spectator.Base;

namespace X.Spectator.Spectators;

[PublicAPI]
public interface IAutomatedSpectator<TState> : ISpectator<TState>, IHostedService
{
}

/// <summary>
/// Automated spectator that periodically checks the health of the system.
/// </summary>
/// <typeparam name="TState"></typeparam>
public class AutomatedSpectator<TState> : SpectatorBase<TState>, IAutomatedSpectator<TState> 
{
    public TimeSpan CheckHealthPeriod { get; }

    private readonly System.Timers.Timer _timer;

    public AutomatedSpectator(
        TimeSpan checkHealthPeriod,
        TimeSpan retentionPeriod,
        IStateEvaluator<TState> stateEvaluator, TState initialState)
        : base(stateEvaluator, retentionPeriod, initialState)
    {
        CheckHealthPeriod = checkHealthPeriod;

        _timer = new System.Timers.Timer(CheckHealthPeriod.TotalMilliseconds);
        _timer.Elapsed += (_, _) => CheckHealth();
        _timer.AutoReset = true;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer.Start();
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Stop();
        
        return Task.CompletedTask;
    }
}