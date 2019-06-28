using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace X.Spectator.Base
{
    public struct JournalRecord
    {
        public JournalRecord(DateTime time, IEnumerable<Record> values)
        {
            Time = time;
            Values = values.ToImmutableList();
        }

        public DateTime Time { get; set; }
        public IReadOnlyCollection<Record> Values { get; set; }

        public override string ToString() => $"{Time}: [{string.Join(",", Values)}]";
    }

    public struct Record
    {
        public string Name { get; set; }
        public bool Value { get; set; }

        public override string ToString() => $"{Name}: {Value}";
    }
}