using Entities;
using Entities.Capacities;
using GameStates;

public class StunPassive : PassiveCapacity
{
    private StunPassiveSO _SOType;
    private float _timer;
    private Champion _champion;
    private float _timeStun;
    
    public override void OnCreate()
    {
        _SOType = (StunPassiveSO)SO;
    }
    
    protected override void OnAddedEffects(Entity target)
    {
        _champion = target.GetComponent<Champion>();
        if (!_champion) return;
        _timeStun = _SOType.Timer;
        GameStateMachine.Instance.OnTick += DecreaseTimerStun;
        if (_champion)
        {
            _champion.RequestSetCanCast(false);
            _champion.RequestSetCurrentMoveSpeed(0);
        }
    }

    protected override void OnAddedFeedbackEffects(Entity target)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnRemovedEffects(Entity target)
    {
        _champion.RequestSetCanCast(true);
        _champion.RequestSetCurrentMoveSpeed(_champion.GetReferenceMoveSpeed());
    }

    protected override void OnRemovedFeedbackEffects(Entity target)
    {
        throw new System.NotImplementedException();
    }

    private void DecreaseTimerStun()
    {
        _timer++;
        if (_timer >= _timeStun * GameStateMachine.Instance.tickRate)
        {
            OnRemoved();
            GameStateMachine.Instance.OnTick -= DecreaseTimerStun;
        }
    }
}