using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using X.Spectator.Base;
using X.Spectator.Spectators;
using Xunit;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace X.Spectator.Tests;

public class Tests
{
    [Fact]
    public async Task TestSpectatorBase()
    {
        var probe1States = new Queue<ProbeResult>(
        [
            C(true), C(true), C(true), C(false), C(true), C(true), C(true), C(true)
        ]);

        var probe2States = new Queue<ProbeResult>(
        [
            C(true), C(true), C(false), C(true), C(false), C(true), C(true), C(true)
        ]);

        IProbe probe1 = new Probe("Test-1", () =>
        {
            var result = probe1States.Dequeue();
            // result.ProbeName = "Test-1";
            return Task.FromResult(result);
        });

        IProbe probe2 = new Probe("Test-2", () =>
        {
            var result = probe2States.Dequeue();
            // result.ProbeName = "Test-2";
            return Task.FromResult(result);
        });

        var stateEvaluatorMock = new Mock<IStateEvaluator<HealthStatus>>();

        stateEvaluatorMock
            .Setup(o => o.Evaluate(
                It.IsAny<HealthStatus>(),
                It.IsAny<DateTime>(),
                It.IsAny<IReadOnlyCollection<JournalRecord>>()))
            .Returns((HealthStatus currentState,
                DateTime stateChangedLastTime,
                IReadOnlyCollection<JournalRecord> journal) =>
            {
                var data = journal.TakeLast(3).ToImmutableList();

                var totalChecks = data.Count;
                var failedChecks = data.Count(o => o.Values.Any(v => v.Value.Status == HealthStatus.Unhealthy));

                if (failedChecks == 0)
                {
                    return HealthStatus.Healthy;
                }

                if (failedChecks == 1)
                {
                    return HealthStatus.Degraded;
                }

                return HealthStatus.Unhealthy;
            });

        IStateEvaluator<HealthStatus> stateEvaluator = stateEvaluatorMock.Object;
        TimeSpan retentionPeriod = TimeSpan.FromMinutes(10);

        var spectator =
            new SpectatorBase<HealthStatus>(stateEvaluator, retentionPeriod, HealthStatus.Unhealthy);

        spectator.AddProbe(probe1);
        spectator.AddProbe(probe2);

        var states = new List<HealthStatus>();

        spectator.HealthChecked += (sender, args) => { };

        spectator.StateChanged += (sender, args) => { states.Add(args.State); };

        for (int i = 0; i < 8; i++)
        {
            spectator.CheckHealth();
        }

        var expected = new[]
        {
            HealthStatus.Healthy,
            HealthStatus.Degraded,
            HealthStatus.Unhealthy,
            HealthStatus.Degraded,
            HealthStatus.Healthy
        };


        Assert.Equal(expected.ToArray(), states.ToArray());
    }

    private ProbeResult C(bool value)
    {
        return new ProbeResult
        {
            Value = value ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy(),
            Time = DateTime.UtcNow,
            ProbeName = "TEST"
        };
    }

    [Fact]
    public async Task TestProbe()
    {
        var probeMock = new Mock<IProbe>();

        probeMock
            .Setup(o => o.Check())
            .Returns(() => Task.FromResult(C(true)));

        var probe = probeMock.Object;

        var result = await probe.Check();

        Assert.True(result.Value.Status == HealthCheckResult.Healthy().Status);
    }
}