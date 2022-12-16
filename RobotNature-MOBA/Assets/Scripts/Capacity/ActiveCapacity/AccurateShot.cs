using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

public class AccurateShot : ActiveCapacity
{
    private Champion champion;
    public AccurateShotSO SOType;
    private Vector3 lookDir;
    private GameObject bullet;
    private AccurateShootCollider collider;
    
    public override void OnStart()
    {
        champion = (Champion)caster;
        SOType = (AccurateShotSO)SO;
    }

    public override void CapacityPress()
    {
        champion.GetPassiveCapacity(SOType.attackAnimationSlowSO).OnAdded();
        champion.OnCastAnimationCast += CapacityEffect;
        champion.OnCastAnimationEnd += CapacityEndAnimation; 
        champion.canRotate = false;
    }

    public override void CapacityEffect(Transform castTransform)
    {
        champion.OnCastAnimationCast -= CapacityEffect;
        lookDir = targetPositions[0]-castTransform.position;
        lookDir.y = 0;
        var shootDir = lookDir;
        bullet = PoolNetworkManager.Instance.PoolInstantiate(SOType.bulletPrefab.GetComponent<Entity>(), castTransform.position, Quaternion.LookRotation(shootDir)).gameObject;
        collider = bullet.GetComponent<AccurateShootCollider>();
        collider.caster = caster;
        collider.casterPos = castTransform.position;
        collider.maxDistance = SOType.maxRange;
        collider.capacitySender = this;
        collider.Launch(shootDir.normalized*SOType.bulletSpeed);
    }

    public override void CollideEntityEffect(Entity entityAffect)
    {
        
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"touch {entityAffect.gameObject.name}");
            if (caster.team == entityAffect.team)
            {
                collider.maxDistance++;
                //Debug.Log("Add 1 to maxDistance");
            }
            else
            {
                Debug.Log("Hit enemi");
                var lifeable = entityAffect.GetComponent<IActiveLifeable>();
                if (lifeable != null)
                {
                    if (!lifeable.AttackAffected()) return;
                    Debug.Log($"Deal damage to {entityAffect.gameObject}");
                    entityAffect.photonView.RPC("DecreaseCurrentHpRPC", RpcTarget.All,
                        caster.GetComponent<Champion>().attackDamage * SOType.percentageDamage/100);
                    collider.Disable();
                }
                
                var moveable = entityAffect.GetComponent<IMovable>();
                if (moveable != null)
                {
                    Debug.Log($"Slow {entityAffect.gameObject}");
                      entityAffect.GetPassiveCapacity(SOType.SlowEffectSO).OnAdded();
                }
            }
        }
    }

    

    public override void CollideObjectEffect(GameObject obj)
    {
        if (obj.CompareTag("Obstacle"))
        {
            collider.EnterWall(obj);
            //Debug.Log($"Enter {obj.name}");
        }
    }

    public override void CollideExitEffect(GameObject obj)
    {
        collider.ExitWall(obj);
        //Debug.Log($"Exit {obj.name}");
    }
    
    public override void CapacityEndAnimation()
    {
        champion.OnCastAnimationEnd -= CapacityEndAnimation;
        champion.GetPassiveCapacity(CapacitySOCollectionManager.GetPassiveCapacitySOIndex(SOType.attackAnimationSlowSO)).OnRemoved();
        champion.canRotate = true;
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
      
    }
}
