using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using X.Spectator.Base;

namespace X.Spectator.StateEvaluators;

[PublicAPI]
public abstract class BooleanStateEvaluators : IStateEvaluator<bool>
{
    public abstract bool Evaluate(
        bool currentState, 
        DateTime stateChangedLastTime,
        IReadOnlyCollection<JournalRecord> journal);
}