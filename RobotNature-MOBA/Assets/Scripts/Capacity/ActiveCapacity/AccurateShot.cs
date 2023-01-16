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
        casterTransform = caster.transform;
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
        lookDir = targetPositions[0]-casterTransform.position;
        lookDir.y = 0;
        bullet = PoolNetworkManager.Instance.PoolInstantiate(SOType.bulletPrefab.GetComponent<Entity>(), casterTransform.position, Quaternion.LookRotation(lookDir)).gameObject;
        collider = bullet.GetComponent<AccurateShootCollider>();
        collider.caster = caster;
        collider.casterPos = casterTransform.position;
        collider.maxDistance = SOType.maxRange;
        collider.capacitySender = this;
        collider.Launch(lookDir.normalized*SOType.bulletSpeed);
    }

    public override void CollideEntityEffect(Entity entity)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"touch {entity.gameObject.name}");
            if (caster.team == entity.team)
            {
                collider.maxDistance++;
            }
            else
            {
                Debug.Log("Hit enemi");
                var lifeable = entity.GetComponent<IActiveLifeable>();
                if (lifeable == null) return;
                if (!lifeable.AttackAffected()) return;
                Debug.Log($"Deal damage to {entity.gameObject}");
                entity.photonView.RPC("DecreaseCurrentHpRPC", RpcTarget.All, caster.GetComponent<Champion>().attackDamage * SOType.percentageDamage/100);
                collider.Disable();

                var moveable = entity.GetComponent<IMovable>();
                if (moveable == null) return;
                Debug.Log($"Slow {entity.gameObject}");
                entity.GetPassiveCapacity(SOType.SlowEffectSO).OnAdded();
            }
        }
    }
    
    public override void CollideObjectEffect(GameObject obj)
    {
        if (obj.CompareTag("Obstacle"))
        {
            collider.EnterWall(obj);
        }
    }

    public override void CollideExitEffect(GameObject obj)
    {
        collider.ExitWall(obj);
    }
    
    public override void CapacityEndAnimation()
    {
        champion.OnCastAnimationEnd -= CapacityEndAnimation;
        champion.GetPassiveCapacity(CapacitySOCollectionManager.GetPassiveCapacitySOIndex(SOType.attackAnimationSlowSO)).OnRemoved();
        champion.canRotate = true;
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions) { }
}