using Entities;
using Entities.Capacities;
using Entities.Minion;
using GameStates;
using UnityEngine;

public class AutoMinion : ActiveCapacity
{
    public AutoMinionSO SOType;
       
    private Entity target;
    private GameObject projectileGO;
    private Minion minion;
    private double timer;

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
        if (Vector3.Distance(target.transform.position, target.transform.position) > minion.attackAbility.maxRange) return false;
        ApplyEffect();
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
        // if (Vector3.Distance(target.transform.position, casterTransform.position) < minion.attackAbility.maxRange)
        // {
        //     IActiveLifeable entityActiveLifeable = target.GetComponent<IActiveLifeable>();
        //     entityActiveLifeable.RequestDecreaseCurrentHp(minion.attackDamage);
        // }
    }

    public override void CapacityEndAnimation()
    {
        // minion.OnCastAnimationEnd -= CapacityEndAnimation;
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions) { }

    private void ApplyEffect()
    {
        if (Vector3.Distance(target.transform.position, casterTransform.position) < minion.attackAbility.maxRange)
        {
            IActiveLifeable entityActiveLifeable = target.GetComponent<IActiveLifeable>();
            entityActiveLifeable.RequestDecreaseCurrentHp(minion.attackDamage);
        }
    }
}