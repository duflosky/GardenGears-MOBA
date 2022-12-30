using Entities;
using Entities.Capacities;
using Entities.Minion;
using Photon.Pun;
using UnityEngine;

public class AutoMinion : ActiveCapacity
{
    [SerializeField] private AutoMinionSO SOType;
       
    private Entity target;
    private Minion minion;
    private GameObject projectileGO;
    private AutoMinionCollider autoMinionCollider;

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
        target = minion.currentAttackTarget.GetComponent<Entity>();
        return base.TryCast(targetsEntityIndexes, targetPositions);
    }

    public override void CapacityPress()
    {
        CapacityEffect(casterTransform);
        // minion.OnCastAnimationCast += CapacityEffect;
        // minion.OnCastAnimationEnd += CapacityEndAnimation;
    }

    public override void CapacityEffect(Transform castTransform)
    {
        projectileGO = PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackPrefab, casterTransform.position, casterTransform.rotation);
        autoMinionCollider = projectileGO.GetComponent<AutoMinionCollider>();
        autoMinionCollider.capacitySender = this;
        autoMinionCollider.caster = caster;
        autoMinionCollider.target = target;
    }
    
    public override void CollideEntityEffect(Entity entityAffect)
    {
        var lifeable = entityAffect.GetComponent<IActiveLifeable>();
        if (lifeable == null) return;
        if (!lifeable.AttackAffected()) return;
        lifeable.RequestDecreaseCurrentHp(minion.attackDamage);
        autoMinionCollider.Disable();
    }

    public override void CollideFeedbackEffect(Entity entityAffect)
    {
        autoMinionCollider.Disable();
    }

    public override void CapacityEndAnimation()
    {
        // minion.OnCastAnimationEnd -= CapacityEndAnimation;
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (PhotonNetwork.IsMasterClient) return;
        projectileGO = PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackPrefab, minion.transform.position, minion.transform.rotation);
        autoMinionCollider = projectileGO.GetComponent<AutoMinionCollider>();
        autoMinionCollider.capacitySender = this;
        autoMinionCollider.caster = caster;
        autoMinionCollider.target = EntityCollectionManager.GetEntityByIndex(targetsEntityIndexes[0]);
    }
}