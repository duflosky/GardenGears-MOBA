using Entities;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

public class AutoAttackRange : ChampionActiveCapacity
{
    private AutoAttackRangeSO SOType;
    private GameObject bullet;
    private AffectCollider collider;
    private GameObject shotGizmo;

    public override void OnStart()
    {
        base.OnStart();
        SOType = (AutoAttackRangeSO)SO;
    }

    public override bool TryCast(int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (!base.TryCast(targetsEntityIndexes, targetPositions)) return false;
        champion.isCasting = true;
        return true;
    }
    
    public override void CapacityShotEffect(Transform transform)
    {
        champion.OnCastAnimationShotEffect -= CapacityShotEffect;
        var direction = casterTransform.GetChild(0).forward;
        PoolLocalManager.Instance.PoolInstantiate(SOType.shotPrefab, transform.position, Quaternion.LookRotation(-direction));
    }

    public override void CapacityEffect(Transform castTransform) { }

    public override void CapacityEndAnimation()
    {
        champion.OnCastAnimationEnd -= CapacityEndAnimation;
        champion.GetPassiveCapacity(SOType.attackSlowSO).OnRemoved();
        champion.isCasting = false;
        champion.canRotate = true;
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        champion.canRotate = false;
        champion.OnCastAnimationCast -= CapacityEffect;
        champion.OnCastAnimationCastFeedback -= PlayFeedback;
        champion.GetPassiveCapacity(SOType.overheatSO).OnAdded();
        float rotateAngle = champion.isOverheat ? Random.Range(-(SOType.sprayAngle / 2), (SOType.sprayAngle / 2)) : 0f;
        var bulletPref = champion.isOverheat ? SOType.overheatBulletPrefab : SOType.bulletPrefab;
        bullet = PoolLocalManager.Instance.PoolInstantiate(bulletPref, casterTransform.position, casterTransform.GetChild(0).rotation);
        collider = bullet.GetComponent<AffectCollider>();
        collider.caster = caster;
        collider.casterPos = casterTransform.position;
        if (champion.isOverheat) collider.transform.Rotate(new Vector3(0,rotateAngle,0));
        collider.maxDistance = SOType.maxRange;
        collider.capacitySender = this;
        collider.Launch(collider.transform.forward*SOType.bulletSpeed);
    }

    public override void CollideEntityEffect(Entity affectedEntity)
    {
        if (caster.team == affectedEntity.team)
        {
            AllyHit(indexOfSOInCollection);
            return;
        }
        var liveable = affectedEntity.GetComponent<IActiveLifeable>();
        if (liveable == null) return;
        if (!liveable.AttackAffected()) return;
        var capacityIndex = CapacitySOCollectionManager.GetActiveCapacitySOIndex(SOType);
        affectedEntity.photonView.RPC("DecreaseCurrentHpByCapacityRPC", RpcTarget.All, caster.GetComponent<Champion>().attackDamage * SOType.percentageDamage, capacityIndex);
        PoolLocalManager.Instance.PoolInstantiate(champion.isOverheat ? SOType.hitOverheatPrefab : SOType.feedbackHitPrefab, affectedEntity.transform.position, Quaternion.identity);
        collider.Disable();
    }

    public override void CollideObjectEffect(GameObject obj)
    {
        if (obj.GetComponent<CaptureZone>()) return; 
        collider.Disable();
    }
}