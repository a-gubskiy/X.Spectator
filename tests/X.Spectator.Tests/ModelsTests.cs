using System;
using Xunit;
using X.Spectator.Base;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace X.Spectator.Tests;

public class ModelsTests
{
    [Fact]
    public void ProbeResult_ToString_Includes_Time_And_Value()
    {
        var probeResult = new ProbeResult
        {
            ProbeName = "p",
            Time = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Value = HealthCheckResult.Healthy()
        };

        var prStr = probeResult.ToString();

        Assert.Contains(probeResult.Value.ToString(), prStr);
        Assert.Contains(probeResult.Time.ToString(), prStr);
    }

    [Fact]
    public void JournalRecord_ToString_Includes_Time_And_Values()
    {
        var probeResult = new ProbeResult
        {
            ProbeName = "p",
            Time = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Value = HealthCheckResult.Healthy()
        };

        var journal = new JournalRecord(probeResult.Time, new[] { probeResult });
        var jrStr = journal.ToString();

        Assert.Contains(probeResult.ToString(), jrStr);
        Assert.Contains(journal.Time.ToString(), jrStr);
    }
}