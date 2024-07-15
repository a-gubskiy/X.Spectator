using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace X.Spectator.Base;

public class JournalRecord
{
    public JournalRecord(DateTime time, IEnumerable<ProbeResult> values)
    {
        Time = time;
        Values = values.ToImmutableList();
    }

    public DateTime Time { get; set; }
        
    public IReadOnlyCollection<ProbeResult> Values { get; set; }

    public override string ToString() => $"{Time}: [{string.Join(",", Values)}]";
}