using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using UnityEngine;

public class AutoAttackRange : ActiveCapacity
{
    
    public override bool TryCast(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if(!base.TryCast(casterIndex, targetsEntityIndexes, targetPositions)) return false;

        Debug.Log("AA Range");
        return true;
    }
    
    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        throw new System.NotImplementedException();
    }
}
