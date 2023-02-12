using Entities;
using Entities.Capacities;
using GameStates;
using Photon.Pun;
using UnityEngine;

public class AutoAttackMelee : ChampionActiveCapacity
{
    private GameObject feedbackObject;
    private AutoAttackMeleeSO SOType;
    private double timer;
    private Vector3 lookDir;

    public override void OnStart()
    {
        base.OnStart();
        SOType = (AutoAttackMeleeSO)ActiveCapacitySO;
    }

    public override void CapacityPress()
    {
        base.CapacityPress();
        Champion.OnCastAnimationFeedback += AnimationFeedback;
    }

    public override void CapacityEffect(Transform castTransform)
    {
        Champion.OnCastAnimationCast -= CapacityEffect;
        lookDir = CasterTransform.GetChild(0).forward;
        lookDir.y = 0;
        feedbackObject = PoolLocalManager.Instance.PoolInstantiate(SOType.damageZone, CasterTransform.position,
            Quaternion.LookRotation(lookDir));
        var affectCollider = feedbackObject.GetComponent<AffectCollider>();
        affectCollider.GetComponent<SphereCollider>().radius = SOType.maxRange;
        affectCollider.Capacity = this;
        affectCollider.Caster = Caster;
        GameStateMachine.Instance.OnTick += DisableObject;
    }

    public override void CollideEntityEffect(Entity entity)
    {
        if (Caster.team == entity.team)
        {
            AllyHit(IndexOfSOInCollection);
            return;
        }

        var liveable = entity.GetComponent<IActiveLifeable>();
        var capacityIndex = CapacitySOCollectionManager.GetActiveCapacitySOIndex(SOType);
        if (liveable != null)
        {
            var angle = Vector3.Angle(lookDir.normalized,
                (entity.transform.position - CasterTransform.position).normalized);
            if (liveable.AttackAffected())
            {
                if (angle > SOType.normalAmplitude) return;
                var hitPos = entity.transform.position +
                             (entity.transform.position - CasterTransform.position).normalized * .5f;
                if (angle <= SOType.perfectAmplitude)
                {
                    PoolLocalManager.Instance.RequestPoolInstantiate(SOType.criticalHitPrefab, hitPos,
                        Quaternion.identity, null, 1f);
                    entity.photonView.RPC("DecreaseCurrentHpByCapacityRPC", RpcTarget.All,
                        Caster.GetComponent<Champion>().attackDamage * SOType.percentageDamageCrit, capacityIndex);
                }
                else
                {
                    PoolLocalManager.Instance.RequestPoolInstantiate(SOType.hitPrefab, hitPos, Quaternion.identity,
                        null, 1f);
                    liveable.DecreaseCurrentHpByCapacityRPC(
                        Caster.GetComponent<Champion>().attackDamage * SOType.percentageDamage, capacityIndex);
                }
            }
        }
    }

    public override void CapacityEndAnimation()
    {
        Champion.GetPassiveCapacity(SOType.attackSlowSO).OnRemoved();
        Champion.OnCastAnimationEnd -= CapacityEndAnimation;
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        Champion.OnCastAnimationCastFeedback -= PlayFeedback;
        lookDir = -CasterTransform.GetChild(0).forward;
        lookDir.y = 0;
        PoolLocalManager.Instance.PoolInstantiate(SOType.fxPrefab, CasterTransform.position,
            Quaternion.LookRotation(lookDir));
    }

    public override void AnimationFeedback()
    {
        lookDir = CasterTransform.GetChild(0).forward;
        lookDir.y = 0;
        feedbackObject = PoolLocalManager.Instance.PoolInstantiate(SOType.fxPrefab, CasterTransform.position,
            Quaternion.LookRotation(-lookDir), CasterTransform);
        var col = feedbackObject.GetComponent<Collider>();
        if (col) col.enabled = false;
        GameStateMachine.Instance.OnTick += DisableObject;
        Champion.OnCastAnimationFeedback -= AnimationFeedback;
    }


    private void DisableObject()
    {
        timer += 1;
        if (timer < 1.5f * GameStateMachine.Instance.tickRate) return;
        GameStateMachine.Instance.OnTick -= DisableObject;
        timer = 0;
        if (feedbackObject) feedbackObject.SetActive(false);
    }
}