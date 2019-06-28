using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;

namespace X.Spectator.Base
{
    public class HealthCheckEventArgs : EventArgs
    {
        public DateTime TimeStamp { get; }

        /// <summary>
        /// Health check results
        /// </summary>
        [NotNull]
        public IReadOnlyCollection<Record> Results { get; }

        public HealthCheckEventArgs(DateTime timeStamp, IReadOnlyCollection<Record> results)
        {
            TimeStamp = timeStamp;
            Results = results ?? ImmutableList<Record>.Empty;
        }
    }
}