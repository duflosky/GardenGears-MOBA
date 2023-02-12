using Entities;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

public class AccurateShot : ChampionActiveCapacity
{
    private AccurateShotSO _accurateShotSO;
    private AccurateShootCollider _accurateShootCollider;
    
    public override void OnStart()
    {
        base.OnStart();
        _accurateShotSO = (AccurateShotSO)ActiveCapacitySO;
    }
    
    public override void CapacityShotEffect(Transform transform)
    {
        Champion.OnCastAnimationShotEffect -= CapacityShotEffect;
        var direction = CasterTransform.GetChild(0).forward;
        PoolLocalManager.Instance.PoolInstantiate(_accurateShotSO.shotPrefab, transform.position, Quaternion.LookRotation(-direction));
    }

    public override void CapacityEffect(Transform castTransform) { }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        Champion.canRotate = false;
        Champion.OnCastAnimationCast -= CapacityEffect;
        Champion.OnCastAnimationCastFeedback -= PlayFeedback;
        var direction = CasterTransform.GetChild(0).forward;
        direction.y = 0;
        var projectile = PoolLocalManager.Instance.PoolInstantiate(_accurateShotSO.bulletPrefab, CasterTransform.position, Quaternion.LookRotation(direction)).gameObject;
        _accurateShootCollider = projectile.GetComponent<AccurateShootCollider>();
        _accurateShootCollider.Caster = Caster;
        _accurateShootCollider.CasterPosition = CasterTransform.position;
        _accurateShootCollider.MaxRange = _accurateShotSO.maxRange;
        _accurateShootCollider.Capacity = this;
        _accurateShootCollider.Launch(direction.normalized*_accurateShotSO.bulletSpeed);
    }

    public override void CollideEntityEffect(Entity entity)
    {
        if (Caster.team == entity.team)
        {
            _accurateShootCollider.MaxRange++;
            AllyHit(IndexOfSOInCollection);
        }
        else
        {
            var lifeable = entity.GetComponent<IActiveLifeable>();
            if (lifeable == null) return;
            if (!lifeable.AttackAffected()) return;
            var capacityIndex = CapacitySOCollectionManager.GetActiveCapacitySOIndex(_accurateShotSO);
            if (PhotonNetwork.IsMasterClient) entity.photonView.RPC("DecreaseCurrentHpByCapacityRPC", RpcTarget.All, Caster.GetComponent<Champion>().attackDamage * _accurateShotSO.percentageDamage/100, capacityIndex);
            PoolLocalManager.Instance.RequestPoolInstantiate(_accurateShotSO.feedbackHitPrefab, entity.transform.position, Quaternion.identity);
            _accurateShootCollider.Disable();
            var moveable = entity.GetComponent<IMovable>();
            if (moveable == null) return;
            if(entity is Champion) entity.GetPassiveCapacity(_accurateShotSO.SlowEffectSO).OnAdded();
        }
    }
    
    public override void CollideObjectEffect(GameObject obj)
    {
        if (obj.CompareTag("Obstacle"))
        {
            _accurateShootCollider.EnterWall(obj);
        }
    }

    public override void CollideExitEffect(GameObject obj)
    {
        _accurateShootCollider.ExitWall(obj);
    }
    
    public override void CapacityEndAnimation()
    {
        Champion.OnCastAnimationEnd -= CapacityEndAnimation;
        Champion.GetPassiveCapacity(CapacitySOCollectionManager.GetPassiveCapacitySOIndex(_accurateShotSO.attackAnimationSlowSO)).OnRemoved();
        Champion.canRotate = true;
    }
}