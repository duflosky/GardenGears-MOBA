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
        SOType = (AutoTowerSO)SO;
        casterTransform = caster.transform;
    }

    public override bool TryCast(int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (targetsEntityIndexes.Length == 0) return false;
        target = EntityCollectionManager.GetEntityByIndex(targetsEntityIndexes[0]);
        tower = caster.GetComponent<Tower>();
        return base.TryCast(targetsEntityIndexes, targetPositions);
    }

    public override void CapacityPress()
    {
        CapacityEffect(casterTransform);
    }

    public override void CapacityEffect(Transform castTransform)
    {
        autoTowerGO = PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackPrefab, tower.shootSpot.position, castTransform.rotation);
        autoTowerCollider = autoTowerGO.GetComponent<AutoTowerCollider>();
        autoTowerCollider.capacitySender = this;
        autoTowerCollider.caster = caster;
        autoTowerCollider.target = target;
        autoTowerGO.transform.LookAt(target.transform);
    }
    
    public override void CollideEntityEffect(Entity entityAffect)
    {
        var lifeable = entityAffect.GetComponent<IActiveLifeable>();
        if (lifeable == null) return;
        if (!lifeable.AttackAffected()) return;
        lifeable.RequestDecreaseCurrentHp(tower.damage);
        autoTowerCollider.Disable();
    }
    
    public override void CollideFeedbackEffect(Entity entityAffect)
    {
        autoTowerCollider.Disable();
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (PhotonNetwork.IsMasterClient) return;
        Tower towerFeedback = caster.GetComponent<Tower>();
        autoTowerGO = PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackPrefab, towerFeedback.shootSpot.position, towerFeedback.shootSpot.rotation);
        autoTowerCollider = autoTowerGO.GetComponent<AutoTowerCollider>();
        autoTowerCollider.capacitySender = this;
        autoTowerCollider.caster = caster;
        autoTowerCollider.target = EntityCollectionManager.GetEntityByIndex(targetsEntityIndexes[0]);
    }
}