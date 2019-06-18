using System;
using System.Collections.Generic;

namespace X.Monitor.Base
{
    public interface IStateChangingRule
    {
        bool ShouldSwitchToDanger(DateTime stateChangedDate, IEnumerable<(DateTime date, IDictionary<string, bool> results)> stateRecords);
        bool ShouldSwitchToNormal(DateTime stateChangedDate, IEnumerable<(DateTime date, IDictionary<string, bool> results)> stateRecords);
    }
    
}