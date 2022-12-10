using Entities;
using Entities.Capacities;
using Entities.Minion;
using GameStates;
using UnityEngine;

public class AutoMinion : ActiveCapacity
{
    private Entity target;
    private Minion minion;
    private double timer;

    public override void OnStart() { }

    public override bool TryCast(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        minion = caster.GetComponent<Minion>();
        if (minion.currentAttackTarget == null) return false;
        target = minion.currentAttackTarget.GetComponent<Entity>();
        
        if (Vector3.Distance(minion.transform.position, target.transform.position) > minion.attackRange) return false;
        
        GameStateMachine.Instance.OnTick += DelayWaitingTick;
        
        return true;
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions) { }
    
    private void DelayWaitingTick()
    {
        timer += 1 / GameStateMachine.Instance.tickRate;

        if (timer >= minion.delayBeforeAttack) 
        {
            ApplyEffect();
            GameStateMachine.Instance.OnTick -= DelayWaitingTick;
        }
    }

    private void ApplyEffect()
    {
        if (Vector3.Distance(target.transform.position, minion.transform.position) < minion.attackRange)
        {
            IActiveLifeable entityActiveLifeable = target.GetComponent<IActiveLifeable>();
            entityActiveLifeable.RequestDecreaseCurrentHp(minion.attackDamage);
        }
    }
}