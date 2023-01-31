﻿using Entities;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

public class UltimateRange : ChampionActiveCapacity
{
    private GameObject ultimateGO;
    private UltimateRangeCollider collider;
    private UltimateRangeSO SOType;
    private Vector3 direction;

    public override void OnStart()
    {
        base.OnStart();
        SOType = (UltimateRangeSO)SO;
    }

    public override bool TryCast(int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (!base.TryCast(targetsEntityIndexes, targetPositions)) return false;
        return true;
    }

    /*public override void CapacityPress()
    {
        champion.OnCastAnimationCast += CapacityEffect;
        champion.OnCastAnimationEnd += CapacityEndAnimation;
        champion.canRotate = false;
    }*/

    public override void CapacityShotEffect(Transform transform)
    {
        champion.OnCastAnimationShotEffect -= CapacityShotEffect;
        direction = targetPositions[0] - casterTransform.position;
        direction.y = 0;
        PoolLocalManager.Instance.RequestPoolInstantiate(SOType.shotPrefab, transform.position, Quaternion.LookRotation(-direction));
    }

    public override void CapacityEffect(Transform transform)
    {
        champion.OnCastAnimationCast -= CapacityEffect;
        direction = targetPositions[0] - casterTransform.position;
        direction.y = 0;
        PoolLocalManager.Instance.PoolInstantiate(SOType.shotPrefab, casterTransform.position, Quaternion.LookRotation(-direction));
        ultimateGO = PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackPrefab, casterTransform.position, Quaternion.LookRotation(-direction));
        collider = ultimateGO.GetComponent<UltimateRangeCollider>();
        //Collider.ActiveDelay(1.2f);
        collider.GetComponent<SphereCollider>().radius = SOType.colliderRadius;
        collider.caster = caster;
        collider.range = SOType.maxRange;
        collider.capacity = this;
        collider.Launch(direction.normalized * SOType.speed);
    }
    
    public override void CapacityEndAnimation()
    {
        champion.OnCastAnimationEnd -= CapacityEndAnimation;
        champion.GetPassiveCapacity(CapacitySOCollectionManager.GetPassiveCapacitySOIndex(ChampSO.capacitySlow)).OnRemoved();
        champion.canRotate = true;
    }
    
    public override void CollideEntityEffect(Entity entity)
    {
        if (caster.team != entity.team)
        {
            var liveable = entity.GetComponent<IActiveLifeable>();
            if (liveable == null) return;
            if (!liveable.AttackAffected()) return;
            entity.photonView.RPC("DecreaseCurrentHpRPC", RpcTarget.All, champion.attackDamage * SOType.damagePercentage);
            collider.SyncDisableRPC();
        }
        else if (caster.team == entity.team)
        {
            AllyHit(indexOfSOInCollection);
            Explode(entity.transform.position);
        }
    }
    
    public override void CollideObjectEffect(GameObject objectAffect)
    {
        collider.SyncDisableRPC();
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (PhotonNetwork.IsMasterClient) return;
        champion.OnCastAnimationCast -= CapacityEffect;
        direction = targetPositions[0] - caster.transform.position;
        direction.y = 0;
        PoolLocalManager.Instance.PoolInstantiate(SOType.shotPrefab, champion.transform.position, Quaternion.identity);
        ultimateGO = PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackPrefab, champion.transform.position, Quaternion.LookRotation(-direction));
        collider = ultimateGO.GetComponent<UltimateRangeCollider>();
        collider.GetComponent<SphereCollider>().radius = SOType.colliderRadius;
        collider.caster = caster;
        collider.range = SOType.maxRange;
        collider.capacity = this;
        collider.Launch(direction.normalized * SOType.speed);
    }
    
    private void Explode(Vector3 position)
    {
        ultimateGO.SetActive(false);
        PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackHitPrefab, position, Quaternion.identity);
        var capacityIndex = CapacitySOCollectionManager.GetActiveCapacitySOIndex(SOType);
        var entities = Physics.OverlapSphere(position, SOType.explosionRadius);
        foreach (var entity in entities)
        {
            var affectedEntity = entity.GetComponent<Entity>();
            if (affectedEntity == null || caster.team == affectedEntity.team) continue;
            var liveableEntity = entity.GetComponent<IActiveLifeable>();
            if (liveableEntity == null || !liveableEntity.AttackAffected()) continue;
            liveableEntity.RequestDecreaseCurrentHpByCapacity(caster.GetComponent<Champion>().attackDamage * SOType.damagePercentage, capacityIndex);
        }
    }
}
