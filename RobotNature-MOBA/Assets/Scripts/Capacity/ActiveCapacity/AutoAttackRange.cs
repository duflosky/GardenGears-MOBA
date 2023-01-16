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

    public override bool TryCast(int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if(!base.TryCast(targetsEntityIndexes, targetPositions)) return false;
        return true;
    }

    public override void CapacityPress()
    {
        champion.GetPassiveCapacity(CapacitySOCollectionManager.GetPassiveCapacitySOIndex(SOType.attackSlowSO)).OnAdded();
        champion.OnCastAnimationCast += CapacityEffect;
        champion.OnCastAnimationEnd += CapacityEndAnimation; 
        champion.canRotate = false;
    }

    public override void CapacityEffect(Transform castTransform)
    {
        champion.OnCastAnimationCast -= CapacityEffect;
        champion.GetPassiveCapacity(CapacitySOCollectionManager.GetPassiveCapacitySOIndex(SOType.overheatSO)).OnAdded();
        lookDir = targetPositions[0]-casterTransform.position;
        lookDir.y = 0;
        if (champion.isOverheat)
        {
            var rdm = Random.Range(-(SOType.sprayAngle / 2), (SOType.sprayAngle / 2));
            lookDir += new Vector3(Mathf.Cos(rdm), 0, Mathf.Sin(rdm)).normalized;
        }

        var bulletPref = champion.isOverheat ? SOType.overheatBulletPrefab : SOType.bulletPrefab;
        bullet = PoolNetworkManager.Instance.PoolInstantiate(bulletPref.GetComponent<Entity>(), casterTransform.position, Quaternion.LookRotation(lookDir)).gameObject;
        collider = bullet.GetComponent<AffectCollider>();
        collider.caster = caster;
        collider.casterPos = casterTransform.position;
        collider.maxDistance = SOType.maxRange;
        collider.capacitySender = this;
        collider.Launch(lookDir.normalized*SOType.bulletSpeed);
    }

    public override void CapacityEndAnimation()
    {
        champion.OnCastAnimationEnd -= CapacityEndAnimation;
        champion.GetPassiveCapacity(CapacitySOCollectionManager.GetPassiveCapacitySOIndex(SOType.attackSlowSO)).OnRemoved();
        champion.canRotate = true;
    }

    public override void CollideEntityEffect(Entity entity)
    {
        if (caster.team == entity.team) return;
        var lifeable = entity.GetComponent<IActiveLifeable>();
        if (lifeable == null) return;
        if (!lifeable.AttackAffected()) return;
        entity.photonView.RPC("DecreaseCurrentHpRPC", RpcTarget.All, caster.GetComponent<Champion>().attackDamage * SOType.percentageDamage);
        collider.Disable();
    }

    public override void CollideFeedbackEffect(Entity affectedEntity)
    {
        if (caster.team == affectedEntity.team) return;
        PoolLocalManager.Instance.RequestPoolInstantiate(SOType.feedbackHitPrefab, affectedEntity.transform.position, Quaternion.identity);
    }

    public override void CollideObjectEffect(GameObject obj)
    { 
        collider.Disable();
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions) { }
}