using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using JetBrains.Annotations;

namespace X.Spectator.Base;

/// <summary>
/// Represents a journal record.
/// </summary>
[PublicAPI]
public class JournalRecord
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="values"></param>
    public JournalRecord(DateTime time, IEnumerable<ProbeResult> values)
    {
        Time = time;
        Values = values.ToImmutableList();
    }

    /// <summary>
    /// Time of the journal record.
    /// </summary>
    public DateTime Time { get; set; }
        
    /// <summary>
    /// Values of the journal record.
    /// </summary>
    public IReadOnlyCollection<ProbeResult> Values { get; set; }

    /// <summary>
    /// Returns a string representation of the journal record.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"{Time}: [{string.Join(",", Values)}]";
}