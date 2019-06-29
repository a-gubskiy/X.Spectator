using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using X.Spectator.Base;
using X.Spectator.Spectators;
using Xunit;

namespace X.Spectator.Tests
{
    public class Tests
    {

        [Fact]
        public async Task TestSpectatorBase()
        {
            var probe1States = new Queue<ProbeResult>(new[]
                {C(true), C(true), C(true), C(false), C(true), C(true), C(true), C(true)}
            );

            var probe2States = new Queue<ProbeResult>(new[]
                {C(true), C(true), C(false), C(true), C(false), C(true), C(true), C(true)}
            );

            var stateEvaluatorMock = new Mock<IStateEvaluator<State>>();
            var probe1Mock = new Mock<IProbe>();
            var probe2Mock = new Mock<IProbe>();
            
            probe1Mock
                .Setup(o => o.Ready())
                .Returns(() =>
                {
                    var probeResult = probe1States.Dequeue();
                    
                    return Task.FromResult(probeResult);
                });
            
            probe2Mock
                .Setup(o => o.Ready())
                .Returns(() =>
                {
                    var probeResult = probe2States.Dequeue();
                    
                    return Task.FromResult(probeResult);
                });


            stateEvaluatorMock
                .Setup(o => o.Evaluate(
                    It.IsAny<State>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<IReadOnlyCollection<JournalRecord>>()))
                .Returns((State currentState,
                    DateTime stateChangedLastTime,
                    IReadOnlyCollection<JournalRecord> journal) =>
                {
                    var data = journal.TakeLast(3).ToImmutableList();

                    var totalChecks = data.Count;
                    var failedChecks = data.Count(o => o.Values.Any(v => v.Success == false));

                    if (failedChecks == 0)
                        return State.Live;

                    if (failedChecks == 1)
                        return State.Warning;

                    return State.Down;
                });

            IProbe probe1 = probe1Mock.Object;
            IProbe probe2 = probe2Mock.Object;
            
            IStateEvaluator<State> stateEvaluator = stateEvaluatorMock.Object;
            TimeSpan retentionPeriod = TimeSpan.FromMinutes(10);
            
            var spectator = new SpectatorBase<State>(stateEvaluator, retentionPeriod, State.Unknown);

            
            spectator.AddProbe(probe1);
            spectator.AddProbe(probe2);

            var states = new List<State>();

            spectator.HealthChecked += (sender, args) =>
            {
                
            };
            
            spectator.StateChanged += (sender, args) =>
            {
                states.Add(args.State);
            };

            for (int i = 0; i < 8; i++)
            {
                spectator.CheckHealth();
            }

            var expected = new State[]
            {
                State.Live, State.Warning, State.Down, State.Warning, State.Live
            };

            
            Assert.Equal(expected.ToArray(), states.ToArray());
        }

        private ProbeResult C(bool value)
        {
            return new ProbeResult
            {
                Success = value,
                Data = "",
                Time = DateTime.UtcNow,
                Exception = null,
                ProbeName = "TEST"
            };
        }

        [Fact]
        public async Task TestProbe()
        {
            var probeMock = new Mock<IProbe>();
            
            probeMock
                .Setup(o => o.Ready())
                .Returns(() => Task.FromResult(C(true)));
            
            var probe = probeMock.Object;

            var result = await probe.Ready();
            
            Assert.True(result.Success);
        }
    }
}