using System;
using System.Collections.Generic;
using System.Linq;
using X.Monitor.Base;

namespace X.Monitor
{
    public class SimpleStateChangingRule : IStateChangingRule
    {
        public virtual bool ShouldSwitchToDanger(
            DateTime stateChangedDate, 
            IEnumerable<(DateTime date, IDictionary<string, bool> results)> stateRecords)
        {
            var (date, results) = stateRecords.LastOrDefault();

            if (results != null)
            {
                if (results.Any(o => !o.Value))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual  bool ShouldSwitchToNormal(
            DateTime stateChangedDate, 
            IEnumerable<(DateTime date, IDictionary<string, bool> results)> stateRecords)
        {
            var (date, results) = stateRecords.LastOrDefault();

            if (results != null)
            {
                if (results.All(o => o.Value))
                {
                    return true;
                }
            }

            return true;
        }
    }

    public class ScheduledStateChangingRule : SimpleStateChangingRule
    {
        private readonly TimeSpan _checkHealthPeriod;

        public ScheduledStateChangingRule(TimeSpan checkHealthPeriod) => _checkHealthPeriod = checkHealthPeriod;

        public override bool ShouldSwitchToDanger(
            DateTime stateChangedDate, 
            IEnumerable<(DateTime date, IDictionary<string, bool> results)> stateRecords)
        {
            throw new NotImplementedException();
        }

        public override  bool ShouldSwitchToNormal(
            DateTime stateChangedDate, 
            IEnumerable<(DateTime date, IDictionary<string, bool> results)> stateRecords)
        {
            throw new NotImplementedException();
        }
    }
}