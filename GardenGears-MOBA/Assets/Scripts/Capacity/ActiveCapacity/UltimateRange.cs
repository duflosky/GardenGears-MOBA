using Entities;
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
        SOType = (UltimateRangeSO)ActiveCapacitySO;
    }
    
    public override bool TryCast(int[] targetsEntityIndexes, Vector3[] targetsPositions)
    {
        if (!base.TryCast(targetsEntityIndexes, targetsPositions)) return false;
        Champion.isCasting = true;
        return true;
    }

    public override void CapacityShotEffect(Transform transform)
    {
        Champion.OnCastAnimationShotEffect -= CapacityShotEffect;
        direction = Caster.transform.GetChild(0).forward;
        direction.y = 0;
        PoolLocalManager.Instance.PoolInstantiate(SOType.shotPrefab, transform.position, Quaternion.LookRotation(-direction));
    }

    public override void CapacityEffect(Transform transform) { }
    
    public override void CapacityEndAnimation()
    {
        Champion.OnCastAnimationEnd -= CapacityEndAnimation;
        Champion.GetPassiveCapacity(SOType.CapacitySlowSO).OnRemoved();
        Champion.isCasting = false;
        Champion.canRotate = true;
    }
    
    public override void CollideEntityEffect(Entity entity)
    {
        if (Caster.team != entity.team)
        {
            var liveable = entity.GetComponent<IActiveLifeable>();
            if (liveable == null) return;
            if (!liveable.AttackAffected()) return;
            if (PhotonNetwork.IsMasterClient) entity.photonView.RPC("DecreaseCurrentHpRPC", RpcTarget.All, Champion.attackDamage * SOType.damagePercentage);
            if (entity.GetComponent<Champion>())
            {
                entity.GetPassiveCapacity(SOType.PassiveAfterHit).OnAdded();
            }
            Explode(entity.transform.position);
            collider.Disable();
        }
        else if (Caster.team == entity.team)
        {
            AllyHit(IndexOfSOInCollection);
            Explode(entity.transform.position);
            collider.Disable();
        }
    }
    
    public override void CollideObjectEffect(GameObject objectAffect)
    {
        Explode(objectAffect.transform.position);
        collider.Disable();
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        Champion.OnCastAnimationCast -= CapacityEffect;
        Champion.OnCastAnimationCastFeedback -= PlayFeedback;
        direction = Caster.transform.GetChild(0).forward;
        direction.y = 0;
        ultimateGO = PoolLocalManager.Instance.PoolInstantiate(SOType.feedbackPrefab, Champion.transform.position, Quaternion.LookRotation(-direction));
        collider = ultimateGO.GetComponent<UltimateRangeCollider>();
        collider.GetComponent<SphereCollider>().radius = SOType.colliderRadius;
        collider.caster = Caster;
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
            if (affectedEntity == null || Caster.team == affectedEntity.team) continue;
            var liveableEntity = entity.GetComponent<IActiveLifeable>();
            if (liveableEntity == null || !liveableEntity.AttackAffected()) continue;
            if (PhotonNetwork.IsMasterClient) liveableEntity.RequestDecreaseCurrentHpByCapacity(Caster.GetComponent<Champion>().attackDamage * SOType.damagePercentage, capacityIndex);
        }
    }
}
