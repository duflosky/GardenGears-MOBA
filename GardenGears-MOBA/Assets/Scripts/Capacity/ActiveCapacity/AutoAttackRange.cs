using Entities;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

public class AutoAttackRange : ChampionActiveCapacity
{
    private AutoAttackRangeSO _autoAttackRangeSO;
    private AffectCollider _affectCollider;

    public override void OnStart()
    {
        base.OnStart();
        _autoAttackRangeSO = (AutoAttackRangeSO)ActiveCapacitySO;
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
        var direction = CasterTransform.GetChild(0).forward;
        PoolLocalManager.Instance.PoolInstantiate(_autoAttackRangeSO.shotPrefab, transform.position,
            Quaternion.LookRotation(-direction));
    }

    public override void CapacityEffect(Transform castTransform) { }

    public override void CapacityEndAnimation()
    {
        Champion.OnCastAnimationEnd -= CapacityEndAnimation;
        Champion.GetPassiveCapacity(_autoAttackRangeSO.CapacitySlowSO).OnRemoved();
        Champion.isCasting = false;
        Champion.canRotate = true;
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        Champion.canRotate = false;
        Champion.OnCastAnimationCast -= CapacityEffect;
        Champion.OnCastAnimationCastFeedback -= PlayFeedback;
        Champion.GetPassiveCapacity(_autoAttackRangeSO.OverheatPassiveCapacitySO).OnAdded();
        var rotateAngle = Champion.isOverheat
            ? Random.Range(-(_autoAttackRangeSO.OverheatAngle / 2), (_autoAttackRangeSO.OverheatAngle / 2))
            : 0f;
        var projectileToShoot = Champion.isOverheat
            ? _autoAttackRangeSO.OverheatProjectile
            : _autoAttackRangeSO.feedbackPrefab;
        var projectile = PoolLocalManager.Instance.PoolInstantiate(projectileToShoot, CasterTransform.position,
            CasterTransform.GetChild(0).rotation);
        _affectCollider = projectile.GetComponent<AffectCollider>();
        _affectCollider.Caster = Caster;
        _affectCollider.CasterPosition = CasterTransform.position;
        if (Champion.isOverheat) _affectCollider.transform.Rotate(new Vector3(0, rotateAngle, 0));
        _affectCollider.MaxRange = _autoAttackRangeSO.maxRange;
        _affectCollider.Capacity = this;
        _affectCollider.Launch(_affectCollider.transform.forward * _autoAttackRangeSO.Speed);
    }

    public override void CollideEntityEffect(Entity affectedEntity)
    {
        if (Caster.team == affectedEntity.team)
        {
            AllyHit(IndexOfSOInCollection);
            return;
        }

        var liveable = affectedEntity.GetComponent<IActiveLifeable>();
        if (liveable == null) return;
        if (!liveable.AttackAffected()) return;
        var capacityIndex = CapacitySOCollectionManager.GetActiveCapacitySOIndex(_autoAttackRangeSO);
        if (PhotonNetwork.IsMasterClient) affectedEntity.photonView.RPC("DecreaseCurrentHpByCapacityRPC", RpcTarget.All,
            Caster.GetComponent<Champion>().attackDamage * _autoAttackRangeSO.PercentDamage, capacityIndex);
        PoolLocalManager.Instance.PoolInstantiate(
            Champion.isOverheat ? _autoAttackRangeSO.OverheatHit : _autoAttackRangeSO.feedbackHitPrefab,
            affectedEntity.transform.position, Quaternion.identity);
        _affectCollider.Disable();
    }

    public override void CollideObjectEffect(GameObject obj)
    {
        if (obj.GetComponent<CaptureZone>()) return;
        _affectCollider.Disable();
    }
}