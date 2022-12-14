using Entities;
using Entities.Capacities;
using GameStates;
using UnityEngine;

public class AutoTower : ActiveCapacity
{
    private Entity _target;
    private Tower _tower;
    private double timer;

    public override void OnStart() { }

    public override bool TryCast(int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        _tower = caster.GetComponent<Tower>();
        _target = _tower.enemiesInRange[0].GetComponent<Entity>();
        
        if (Vector3.Distance(_tower.transform.position, _target.transform.position) > _tower.detectionRange){return false;}
        
        GameStateMachine.Instance.OnTick += DelayWaitingTick;
        
        return true;
    }

    public override void CapacityPress()
    {
        throw new System.NotImplementedException();
    }

    public override void CapacityEffect(Transform castTransform)
    {
        throw new System.NotImplementedException();
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions) { }
    
    private void DelayWaitingTick()
    {
        timer += 1 / GameStateMachine.Instance.tickRate;

        if (timer >= _tower.delayBeforeAttack) 
        {
            ApplyEffect();
            GameStateMachine.Instance.OnTick -= DelayWaitingTick;
        }
    }
    
    private void ApplyEffect()
    {
        if (Vector3.Distance(_tower.transform.position, _target.transform.position) < _tower.detectionRange)
        {
            IActiveLifeable entityActiveLifeable = _target.GetComponent<IActiveLifeable>();
            entityActiveLifeable.RequestDecreaseCurrentHp(_tower.damage); 
        }
    }
}