using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Capacities;
using UnityEngine;

public class AutoAttackRange : ActiveCapacity
{
    private AutoAttackRangeSO SOType;
    private Vector3 lookDir;
    private AffectCollider bullet;
    
    public override bool TryCast(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if(!base.TryCast(casterIndex, targetsEntityIndexes, targetPositions)) return false;
        SOType = (AutoAttackRangeSO)SO;
        lookDir = targetPositions[0]-casterTransform.position;
        lookDir.y = 0;
        bullet = PoolLocalManager.Instance.PoolInstantiate(SOType.bulletPrefab, casterTransform.position, Quaternion.LookRotation(lookDir)).GetComponent<AffectCollider>();
        bullet.caster = caster;
        bullet.capacitySender = this;
        bullet.Launch(lookDir*SOType.bulletSpeed);
        Debug.Log("AA Range");
        return true;
    }

    public override void CollideEffect(Entity entityAffect)
    {
        var lifeable = entityAffect.GetComponent<IActiveLifeable>();
        if (lifeable != null)
        {
            if(!lifeable.AttackAffected())return;
            
            lifeable.DecreaseCurrentHpRPC(SOType.bulletDamage);
            bullet.gameObject.SetActive(false);
        }
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        throw new System.NotImplementedException();
    }
}
