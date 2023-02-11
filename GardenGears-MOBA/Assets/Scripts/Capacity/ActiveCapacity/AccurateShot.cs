using Entities;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

public class AccurateShot : ChampionActiveCapacity
{
    public AccurateShotSO SOType;
    private Vector3 lookDir;
    private GameObject bullet;
    private AccurateShootCollider collider;
    
    public override void OnStart()
    {
        base.OnStart();
        SOType = (AccurateShotSO)SO;
    }
    
    public override void CapacityShotEffect(Transform transform)
    {
        champion.OnCastAnimationShotEffect -= CapacityShotEffect;
        var direction = casterTransform.GetChild(0).forward;
        PoolLocalManager.Instance.PoolInstantiate(SOType.shotPrefab, transform.position, Quaternion.LookRotation(-direction));
    }

    public override void CapacityEffect(Transform castTransform) { }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        champion.canRotate = false;
        champion.OnCastAnimationCast -= CapacityEffect;
        champion.OnCastAnimationCastFeedback -= PlayFeedback;
        lookDir = casterTransform.GetChild(0).forward;
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
        if (caster.team == entity.team)
        {
            collider.maxDistance++;
            AllyHit(indexOfSOInCollection);
        }
        else
        {
            var lifeable = entity.GetComponent<IActiveLifeable>();
            if (lifeable == null) return;
            if (!lifeable.AttackAffected()) return;
            var capacityIndex = CapacitySOCollectionManager.GetActiveCapacitySOIndex(SOType);
            if (PhotonNetwork.IsMasterClient) entity.photonView.RPC("DecreaseCurrentHpByCapacityRPC", RpcTarget.All, caster.GetComponent<Champion>().attackDamage * SOType.percentageDamage/100, capacityIndex);
            PoolLocalManager.Instance.RequestPoolInstantiate(SOType.feedbackHitPrefab, entity.transform.position, Quaternion.identity);
            collider.Disable();
            var moveable = entity.GetComponent<IMovable>();
            if (moveable == null) return;
            if(entity is Champion) entity.GetPassiveCapacity(SOType.SlowEffectSO).OnAdded();
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
}