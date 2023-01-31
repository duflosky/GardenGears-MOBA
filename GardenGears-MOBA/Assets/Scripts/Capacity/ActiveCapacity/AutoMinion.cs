using Entities;
using Entities.Capacities;
using Entities.Minion;
using Photon.Pun;
using UnityEngine;

public class AutoMinion : ActiveCapacity
{
    private AutoMinionCollider autoMinionCollider;
    private AutoMinionSO SOType;
    private Entity target;
    private GameObject projectileGO;
    private Minion minion;

    public override void OnStart()
    {
        SOType = (AutoMinionSO)SO;
        casterTransform = caster.transform;
        minion = (Minion)caster;
    }

    public override bool TryCast(int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        minion = caster.GetComponent<Minion>();
        if (minion.currentAttackTarget == null) return false;
        if (!minion.currentAttackTarget.GetComponent<IDeadable>().IsAlive()) return false;
        target = minion.currentAttackTarget.GetComponent<Entity>();
        return base.TryCast(targetsEntityIndexes, targetPositions);
    }

    public override void CapacityPress()
    {
        minion.OnCastAnimationCast += CapacityEffect;
        minion.OnCastAnimationEnd += CapacityEndAnimation;
    }

    public override void CapacityEffect(Transform shootPoint)
    {
        minion.OnCastAnimationCast -= CapacityEffect;
        projectileGO = PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackPrefab, shootPoint.position, shootPoint.rotation);
        autoMinionCollider = projectileGO.GetComponent<AutoMinionCollider>();
        autoMinionCollider.capacity = this;
        autoMinionCollider.caster = caster;
        autoMinionCollider.target = target;
        projectileGO.transform.LookAt(target.transform);
    }
    
    public override void CollideEntityEffect(Entity entity)
    {
        var lifeable = entity.GetComponent<IActiveLifeable>();
        if (lifeable == null) return;
        if (!lifeable.AttackAffected()) return;
        lifeable.RequestDecreaseCurrentHp(minion.attackDamage);
        autoMinionCollider.Disable();
    }

    public override void CollideFeedbackEffect(Entity affectedEntity)
    {
        autoMinionCollider.Disable();
    }

    public override void CapacityEndAnimation()
    {
        minion.OnCastAnimationEnd -= CapacityEndAnimation;
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (PhotonNetwork.IsMasterClient) return;
        projectileGO = PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackPrefab, minion.transform.position, minion.transform.rotation);
        autoMinionCollider = projectileGO.GetComponent<AutoMinionCollider>();
        autoMinionCollider.capacity = this;
        autoMinionCollider.caster = caster;
        autoMinionCollider.target = EntityCollectionManager.GetEntityByIndex(targetsEntityIndexes[0]);
    }
}