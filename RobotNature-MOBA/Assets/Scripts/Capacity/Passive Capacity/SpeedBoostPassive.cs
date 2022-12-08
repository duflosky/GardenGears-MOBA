using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Capacities;
using GameStates;
using MoreMountains.Tools;
using UnityEngine;

public class SpeedBoostPassive : PassiveCapacity
{
    private SpeedBoostPassiveSO SOType;
    private Champion champ;

    private float boost;
    private int timer;

    public override void OnCreate()
    {
        SOType = (SpeedBoostPassiveSO)SO;
        champ = (Champion)entity;
    }

    protected override void OnAddedEffects(Entity target)
    {
        boost = SOType.speedBonus;
        if (SOType.isRatio)
        {
            boost = champ.GetCurrentMoveSpeed()* (SOType.speedBonus/100);
        }
        champ.IncreaseCurrentMoveSpeedRPC(boost);
        if(SOType.duration != 0)GameStateMachine.Instance.OnTick += DecreaseTimer; 
    }

    protected override void OnAddedFeedbackEffects(Entity target)
    {
        throw new System.NotImplementedException();
    }

    
    protected override void OnRemovedEffects(Entity target)
    {
        champ.DecreaseCurrentMoveSpeedRPC(boost);
    }

    protected override void OnRemovedFeedbackEffects(Entity target)
    {
        throw new System.NotImplementedException();
    }

    void DecreaseTimer()
    {
        timer++;
        if (timer >= SOType.duration * GameStateMachine.Instance.tickRate)
        {
            GameStateMachine.Instance.OnTick -= DecreaseTimer; 
            OnRemoved(entity);
        }

    }
}
