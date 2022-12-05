using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Capacities;
using UnityEngine;

public class RangePassif : PassiveCapacity
{
    private RangePassifSO SOType;
    
    
    public override PassiveCapacitySO AssociatedPassiveCapacitySO()
    {
        return (PassiveCapacitySO)SOType;
    }

    protected override void OnAddedEffects(Entity target)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnAddedFeedbackEffects(Entity target)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnRemovedEffects(Entity target)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnRemovedFeedbackEffects(Entity target)
    {
        throw new System.NotImplementedException();
    }
}
