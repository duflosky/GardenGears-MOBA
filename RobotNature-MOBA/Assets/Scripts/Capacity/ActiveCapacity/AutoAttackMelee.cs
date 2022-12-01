using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Capacities;
using GameStates;
using UnityEngine;

public class AutoAttackMelee : ActiveCapacity
{
    private AffectCollider collider;
    private AutoAttackMeleeSO SOType;
    private double timer;

    public override bool TryCast(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if(!base.TryCast(casterIndex, targetsEntityIndexes, targetPositions)) return false;
        SOType = (AutoAttackMeleeSO)SO;
        Transform tr = EntityCollectionManager.GetEntityByIndex(casterIndex).transform;
        Vector3 lookDir = targetPositions[0]-tr.position;
        lookDir.y = 0;
        var zoneGO = PoolLocalManager.Instance.PoolInstantiate(SOType.damageZone, tr.position+lookDir.normalized*.5f, Quaternion.LookRotation(lookDir) );
        collider = zoneGO.GetComponentInChildren<AffectCollider>();
        collider.capacitySender = this;
        GameStateMachine.Instance.OnTick += DisableCollider;
        return true;
    }

    public override void CollideEffect(Entity entityAffect)
    {
        IActiveLifeable lifeable = entityAffect.GetComponent<IActiveLifeable>();
        if (lifeable != null)
        {
            if (lifeable.AttackAffected())
            {
              lifeable.DecreaseCurrentHpRPC(SOType.damageAmount);  
            }
        }
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        
    }

    void DisableCollider()
    {
        timer += 1;
        if(timer < 1.5f*GameStateMachine.Instance.tickRate)return;
        GameStateMachine.Instance.OnTick -= DisableCollider;
        timer = 0;
        collider.gameObject.SetActive(false);
    }
}
