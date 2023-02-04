using Entities;
using Entities.Capacities;
using GameStates;

public class StunPassive : PassiveCapacity
{
    private StunPassiveSO _SOType;
    
    public override void OnCreate()
    {
        _SOType = (StunPassiveSO)SO;
    }
    
    protected override void OnAddedEffects(Entity target)
    {
        GameStateMachine.Instance.OnTick += DecreaseTimerStun;
    }

    protected override void OnAddedFeedbackEffects(Entity target)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnRemovedEffects(Entity target)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnRemovedFeedbackEffects(Entity target)
    {
        throw new System.NotImplementedException();
    }

    private void DecreaseTimerStun()
    {
        
    }
}