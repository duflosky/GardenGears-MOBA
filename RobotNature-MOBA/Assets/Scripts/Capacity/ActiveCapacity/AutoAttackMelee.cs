using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using UnityEngine;

public class AutoAttackMelee : ActiveCapacity
{
    private AffectCollider collider;
    private AutoAttackMeleeSO SOType;

    public override bool TryCast(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if(!base.TryCast(casterIndex, targetsEntityIndexes, targetPositions)) return false;
        SOType = (AutoAttackMeleeSO)SO;
        Debug.Log($"targetPositions {targetPositions}, transform {transform}");
        Vector3 lookDir = targetPositions[0]-transform.position;
        var zoneGO = PoolLocalManager.Instance.PoolInstantiate(SOType.damageZone, transform.position, Quaternion.LookRotation(lookDir) );
        collider = zoneGO.GetComponent<AffectCollider>();
        collider.capacitySender = this;
        StartCoroutine(DisableCollider());
        Debug.Log("AA Melee");
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

    IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(1f);
        collider.gameObject.SetActive(false);
    }
}
