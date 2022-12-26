using Entities;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

public class AutoTower : ActiveCapacity
{
    private Entity target;
    private Tower tower;
    public AutoTowerSO SOType;
    private GameObject autoTowerGO;
    private AutoTowerCollider autoTowerCollider;
    
    public override void OnStart()
    {
        SOType = (AutoTowerSO)SO;
        casterTransform = caster.transform;
    }

    public override bool TryCast(int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (targetsEntityIndexes.Length == 0) return false;
        target = EntityCollectionManager.GetEntityByIndex(targetsEntityIndexes[0]);
        tower = caster.GetComponent<Tower>();
        
        if (Vector3.Distance(casterTransform.position, target.transform.position) > tower.range) return false;
        
        if (!base.TryCast(targetsEntityIndexes, targetPositions)) return false;
        return true;
    }

    public override void CapacityPress()
    {
        CapacityEffect(casterTransform);
    }

    public override void CapacityEffect(Transform castTransform)
    {
        autoTowerGO = PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackPrefab, castTransform.position, castTransform.rotation);
        autoTowerCollider = autoTowerGO.GetComponent<AutoTowerCollider>();
        autoTowerCollider.capacitySender = this;
        autoTowerCollider.caster = caster;
        autoTowerCollider.target = target;
    }
    
    public override void CollideEntityEffect(Entity entityAffect)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (caster.team == entityAffect.team) return;
        var lifeable = entityAffect.GetComponent<IActiveLifeable>();
        if (lifeable == null) return;
        if (!lifeable.AttackAffected()) return;
        entityAffect.photonView.RPC("DecreaseCurrentHpRPC", RpcTarget.All, tower.damage);
        autoTowerCollider.SyncDisableRPC();
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        autoTowerGO = PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackPrefab, casterTransform.position, casterTransform.rotation);
        autoTowerCollider = autoTowerGO.GetComponent<AutoTowerCollider>();
        autoTowerCollider.capacitySender = this;
        autoTowerCollider.caster = caster;
        autoTowerCollider.target = target;
        autoTowerCollider.SyncDisableRPC();
    }
}