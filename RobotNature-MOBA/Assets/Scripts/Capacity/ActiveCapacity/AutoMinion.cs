using Entities;
using Entities.Capacities;
using Entities.Minion;
using GameStates;
using UnityEngine;

public class AutoMinion : ActiveCapacity
{
    private Entity _target;
    private Minion _minion;
    private double timer;

    public override void OnStart()
    {
        Debug.Log("Starting fire");
    }

    public override bool TryCast(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        _minion = caster.GetComponent<Minion>();
        _target = _minion.currentAttackTarget.GetComponent<Entity>();
        
        if (Vector3.Distance(_minion.transform.position, _target.transform.position) > _minion.attackRange){return false;}
        
        GameStateMachine.Instance.OnTick += DelayWaitingTick;
        
        return true;
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
    }
    
    private void DelayWaitingTick()
    {
        timer += 1 / GameStateMachine.Instance.tickRate;

        if (timer >= _minion.delayBeforeAttack) 
        {
            ApplyEffect();
            GameStateMachine.Instance.OnTick -= DelayWaitingTick;
        }
    }

    private void ApplyEffect()
    {
        if (Vector3.Distance(_target.transform.position, _minion.transform.position) < _minion.attackRange)
        {
            IActiveLifeable entityActiveLifeable = _target.GetComponent<IActiveLifeable>();
            entityActiveLifeable.RequestDecreaseCurrentHp(_minion.attackDamage);
        }
    }
}