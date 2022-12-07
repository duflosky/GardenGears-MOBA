using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

public class AutoAttackRange : ActiveCapacity
{
    private Champion champion;
    
    private AutoAttackRangeSO SOType;
    private Vector3 lookDir;
    private GameObject bullet;
    private AffectCollider collider;

    public override void OnStart()
    {
        SOType = (AutoAttackRangeSO)SO;
        casterTransform = caster.transform;
        champion = (Champion)caster;
    }

    public override bool TryCast(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if(!base.TryCast(casterIndex, targetsEntityIndexes, targetPositions)) return false;
        lookDir = targetPositions[0]-casterTransform.position;
        lookDir.y = 0;
        bullet = PoolNetworkManager.Instance.PoolInstantiate(SOType.bulletPrefab.GetComponent<Entity>(), casterTransform.position, Quaternion.LookRotation(lookDir)).gameObject;
        collider = bullet.GetComponent<AffectCollider>();
        collider.caster = caster;
        collider.capacitySender = this;
        var shootDir = lookDir;
        if (champion.isOverheat)
        {
            var rdm = Random.Range(-(SOType.sprayAngle / 2), (SOType.sprayAngle / 2));
            shootDir += new Vector3(Mathf.Cos(rdm), 0, Mathf.Sin(rdm)).normalized;
        };
        collider.Launch(shootDir*SOType.bulletSpeed);
        return true;
    }

    public override void CollideEntityEffect(Entity entityAffect)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            var lifeable = entityAffect.GetComponent<IActiveLifeable>();
            if (lifeable != null)
            {
                if(!lifeable.AttackAffected())return;
                champion.GetPassiveCapacityBySOIndex(CapacitySOCollectionManager.GetPassiveCapacitySOIndex(SOType.overheatSO)).OnAdded(caster,1);
                entityAffect.photonView.RPC("DecreaseCurrentHpRPC", RpcTarget.All, SOType.bulletDamage);
                collider.Disable();
            }
        }
        else
        { 
            bullet.gameObject.SetActive(false);
        }
    }

    public override void CollideObjectEffect(GameObject obj)
    {
       collider.Disable();
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        /*if (PhotonNetwork.IsMasterClient) return;
        lookDir = targetPositions[0]-casterTransform.position;
        lookDir.y = 0;
        bullet = PoolLocalManager.Instance.PoolInstantiate(SOType.bulletPrefab, casterTransform.position, Quaternion.LookRotation(lookDir));
        bullet.GetComponent<AffectCollider>().Launch(lookDir*SOType.bulletSpeed);
        bullet.GetComponent<AffectCollider>().caster = caster;*/
    }
}
