using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using X.Spectator.StateEvaluators;
using X.Spectator.Base;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace X.Spectator.Tests;

public class StateEvaluatorTests
{
    [Fact]
    public void DiscreteStateEvaluator_Constructor_Sets_Values()
    {
        // create a test subclass to expose protected fields
        var evaluator = new TestDiscreteEvaluator(1, 10, 2);

        Assert.Equal(1, evaluator.MinValue);
        Assert.Equal(10, evaluator.MaxValue);
        Assert.Equal(2, evaluator.StepValue);
    }

    [Fact]
    public void BooleanStateEvaluator_Evaluate_Counts_Unhealthy()
    {
        var evaluator = new TestBooleanEvaluator();

        var healthy = CreateProbeResult(true);
        var unhealthy = CreateProbeResult(false);

        // empty journal -> no unhealthy
        var journalEmpty = new List<JournalRecord>();
        Assert.False(evaluator.Evaluate(false, DateTime.UtcNow, journalEmpty));

        // journal with one unhealthy
        var jr = new JournalRecord(DateTime.UtcNow, new[] { healthy, unhealthy });
        var journal = new List<JournalRecord> { jr };

        Assert.True(evaluator.Evaluate(false, DateTime.UtcNow, journal));
    }

    [Fact]
    public void PercentageStateEvaluator_Evaluate_Calculates_Percentage_And_Handles_Empty()
    {
        var evaluator = new TestPercentageEvaluator();

        double currentState = 50.0;

        // empty journal -> returns current state
        var empty = new List<JournalRecord>();
        Assert.Equal(currentState, evaluator.Evaluate(currentState, DateTime.UtcNow, empty));

        // journal with 2 healthy and 1 unhealthy -> 66.666...
        var healthy = CreateProbeResult(true);
        var unhealthy = CreateProbeResult(false);

        var jr = new JournalRecord(DateTime.UtcNow, new[] { healthy, healthy, unhealthy });
        var journal = new List<JournalRecord> { jr };

        var result = evaluator.Evaluate(0.0, DateTime.UtcNow, journal);
        Assert.InRange(result, 66.6, 66.7);
    }

    // Helper test-only implementations
    private class TestDiscreteEvaluator : DiscreteStateEvaluator
    {
        public TestDiscreteEvaluator(int min, int max, int step) : base(min, max, step) { }

        // expose protected fields for assertions
        public int MinValue => Min;
        public int MaxValue => Max;
        public int StepValue => Step;

        public override int Evaluate(int currentState, DateTime stateChangedLastTime, IReadOnlyCollection<JournalRecord> journal) => currentState;
    }

    private class TestBooleanEvaluator : BooleanStateEvaluator
    {
        public override bool Evaluate(bool currentState, DateTime stateChangedLastTime, IReadOnlyCollection<JournalRecord> journal)
        {
            var failed = journal.SelectMany(j => j.Values).Count(v => v.Value.Status == HealthStatus.Unhealthy);
            return failed > 0;
        }
    }

    private class TestPercentageEvaluator : PercentageStateEvaluator
    {
        public override double Evaluate(double currentState, DateTime stateChangedLastTime, IReadOnlyCollection<JournalRecord> journal)
        {
            var all = journal.SelectMany(j => j.Values).ToList();
            if (!all.Any()) return currentState;

            var healthyCount = all.Count(r => r.Value.Status == HealthStatus.Healthy);
            return healthyCount / (double)all.Count * 100.0;
        }
    }

    private static ProbeResult CreateProbeResult(bool healthy) =>
        new()
        {
            ProbeName = "t",
            Time = DateTime.UtcNow,
            Value = healthy ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy()
        };
}