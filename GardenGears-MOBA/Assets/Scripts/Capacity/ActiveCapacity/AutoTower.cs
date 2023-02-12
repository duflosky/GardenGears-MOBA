using Entities;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

public class AutoTower : ActiveCapacity
{
    [SerializeField] private AutoTowerSO SOType;
    
    private Entity target;
    private Tower tower;
    private GameObject autoTowerGO;
    private AutoTowerCollider autoTowerCollider;
    
    public override void OnStart()
    {
        SOType = (AutoTowerSO)ActiveCapacitySO;
        CasterTransform = Caster.transform;
    }

    public override bool TryCast(int[] targetsEntityIndexes, Vector3[] targetsPositions)
    {
        if (targetsEntityIndexes.Length == 0) return false;
        target = EntityCollectionManager.GetEntityByIndex(targetsEntityIndexes[0]);
        tower = Caster.GetComponent<Tower>();
        return base.TryCast(targetsEntityIndexes, targetsPositions);
    }

    public override void CapacityPress()
    {
        CapacityEffect(CasterTransform);
    }

    public override void CapacityEffect(Transform castTransform)
    {
        autoTowerGO = PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackPrefab, tower.shotSpot.position, castTransform.rotation);
        autoTowerCollider = autoTowerGO.GetComponent<AutoTowerCollider>();
        autoTowerCollider.capacitySender = this;
        autoTowerCollider.caster = Caster;
        autoTowerCollider.target = target;
        autoTowerGO.transform.LookAt(target.transform);
    }
    
    public override void CollideEntityEffect(Entity entity)
    {
        var lifeable = entity.GetComponent<IActiveLifeable>();
        if (lifeable == null) return;
        if (!lifeable.AttackAffected()) return;
        lifeable.RequestDecreaseCurrentHp(tower.damage);
        autoTowerCollider.Disable();
    }
    
    public override void CollideFeedbackEffect(Entity affectedEntity)
    {
        autoTowerCollider.Disable();
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (PhotonNetwork.IsMasterClient) return;
        Tower towerFeedback = Caster.GetComponent<Tower>();
        autoTowerGO = PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackPrefab, towerFeedback.shotSpot.position, towerFeedback.shotSpot.rotation);
        autoTowerCollider = autoTowerGO.GetComponent<AutoTowerCollider>();
        autoTowerCollider.capacitySender = this;
        autoTowerCollider.caster = Caster;
        autoTowerCollider.target = EntityCollectionManager.GetEntityByIndex(targetsEntityIndexes[0]);
    }
}