using System;
using System.Threading.Tasks;
using Xunit;
using X.Spectator;
using X.Spectator.Base;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace X.Spectator.Tests
{
    public class ProbeTests
    {
        [Fact]
        public async Task Probe_Check_Returns_Unhealthy_On_Exception()
        {
            var probe = new Probe("boom", () => throw new InvalidOperationException("boom"));

            var result = await probe.Check();

            Assert.Equal("boom", result.ProbeName);
            Assert.Equal(HealthStatus.Unhealthy, result.Value.Status);
            Assert.NotNull(result.Value.Exception);
        }

        [Fact]
        public async Task Probe_Check_Returns_Result_On_Success()
        {
            var expected = new ProbeResult
            {
                ProbeName = "ok",
                Time = DateTime.UtcNow,
                Value = HealthCheckResult.Healthy()
            };

            var probe = new Probe("ok", () => Task.FromResult(expected));

            var result = await probe.Check();

            Assert.Equal(expected.ProbeName, result.ProbeName);
            Assert.Equal(expected.Value.Status, result.Value.Status);
        }
    }
}

