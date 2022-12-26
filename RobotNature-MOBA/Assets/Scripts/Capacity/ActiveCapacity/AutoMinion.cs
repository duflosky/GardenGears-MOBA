using Entities;
using Entities.Capacities;
using Entities.Minion;
using Photon.Pun;
using UnityEngine;

public class AutoMinion : ActiveCapacity
{
    [SerializeField] private AutoMinionSO SOType;
       
    private Entity target;
    private GameObject projectileGO;
    private Minion minion;
    private double timer;
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
        if (!base.TryCast(targetsEntityIndexes, targetPositions)) return false;
        return true;
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
        if (!PhotonNetwork.IsMasterClient) return;
        if (caster.team == entityAffect.team) return;
        var lifeable = entityAffect.GetComponent<IActiveLifeable>();
        if (lifeable == null) return;
        if (!lifeable.AttackAffected()) return;
        entityAffect.photonView.RPC("DecreaseCurrentHpRPC", RpcTarget.All, minion.attackDamage);
        autoMinionCollider.SyncDisableRPC();
    }

    public override void CollideFeedbackEffect(Entity entityAffect)
    {
        if (PhotonNetwork.IsMasterClient) return;
        autoMinionCollider.SyncDisableRPC();
    }

    public override void CapacityEndAnimation()
    {
        // minion.OnCastAnimationEnd -= CapacityEndAnimation;
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        var entity = EntityCollectionManager.GetEntityByIndex(casterIndex);
        if (entity == null) return;
        projectileGO = PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackPrefab, entity.transform.position, entity.transform.rotation);
        autoMinionCollider = projectileGO.GetComponent<AutoMinionCollider>();
        autoMinionCollider.capacitySender = this;
        autoMinionCollider.caster = caster;
        autoMinionCollider.target = target;
        autoMinionCollider.SyncDisableRPC();
    }
}