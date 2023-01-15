using Entities;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

public class UltimateRange : ActiveCapacity
{
    private Champion champion;
    private GameObject ultimateGO;
    private UltimateRangeCollider collider;
    private UltimateRangeSO SOType;
    private Vector3 direction;

    public override void OnStart()
    {
        SOType = (UltimateRangeSO)SO;
        champion = (Champion)caster;
    }

    public override bool TryCast(int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (!base.TryCast(targetsEntityIndexes, targetPositions)) return false;
        return true;
    }

    public override void CapacityPress()
    {
        champion.OnCastAnimationCast += CapacityEffect;
        champion.OnCastAnimationEnd += CapacityEndAnimation;
        champion.canRotate = false;
    }

    public override void CapacityEffect(Transform transform)
    {
        champion.OnCastAnimationCast -= CapacityEffect;
        direction = targetPositions[0] - transform.position;
        direction.y = 0;
        ultimateGO = PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackPrefab, champion.transform.position, Quaternion.identity);
        collider = ultimateGO.GetComponent<UltimateRangeCollider>();
        collider.GetComponent<SphereCollider>().radius = SOType.radius;
        collider.caster = caster;
        collider.range = SOType.maxRange;
        collider.capacity = this;
        collider.Launch(direction.normalized * SOType.speed);
    }
    
    public override void CapacityEndAnimation()
    {
        champion.OnCastAnimationEnd -= CapacityEndAnimation;
        champion.canRotate = true;
    }
    
    public override void CollideEntityEffect(Entity entityAffect)
    {
        if (caster.team == entityAffect.team) return;
        var lifeable = entityAffect.GetComponent<IActiveLifeable>();
        if (lifeable == null) return;
        if (!lifeable.AttackAffected()) return;
        entityAffect.photonView.RPC("DecreaseCurrentHpRPC", RpcTarget.All, champion.attackDamage * SOType.damagePercentage);
        collider.SyncDisableRPC();
    }
    
    public override void CollideFeedbackEffect(Entity entityAffect)
    {
        // TODO: Add feedback on hit
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        champion.OnCastAnimationCast -= CapacityEffect;
        direction = targetPositions[0] - caster.transform.position;
        direction.y = 0;
        ultimateGO = PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackPrefab, champion.transform.position, Quaternion.identity);
        collider = ultimateGO.GetComponent<UltimateRangeCollider>();
        collider.GetComponent<SphereCollider>().radius = SOType.radius;
        collider.caster = caster;
        collider.range = SOType.maxRange;
        collider.capacity = this;
        collider.Launch(direction.normalized * SOType.speed);
    }
}
