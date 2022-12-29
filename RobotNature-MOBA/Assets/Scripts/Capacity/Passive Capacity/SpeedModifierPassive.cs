using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Capacities;
using GameStates;
using MoreMountains.Tools;
using UnityEngine;

public class SpeedModifierPassive : PassiveCapacity
{
    private SpeedModifierPassiveSO SOType;
    private Champion champ;

    private float boost;
    private int timer;

    public override void OnCreate()
    {
        SOType = (SpeedModifierPassiveSO)SO;
        champ = (Champion)entity;
    }

    protected override void OnAddedEffects(Entity target)
    {
        boost = SOType.speedBonus;
        if (SOType.isRatio)
        {
            boost = champ.GetCurrentMoveSpeed()* (SOType.speedBonus/100);
        }

        if (SOType.isBuff) champ.IncreaseCurrentMoveSpeedRPC(boost);
        else
        {
            champ.DecreaseCurrentMoveSpeedRPC(boost);
            Debug.Log($"Slow {champ.gameObject.name} with {SOType.name}");
        }
        if (SOType.duration != 0)
        {
            GameStateMachine.Instance.OnTick += DecreaseTimer;
            Debug.Log($"Add {SOType.name} to OnTick");
        } 
    }

    protected override void OnAddedFeedbackEffects(Entity target)
    {
        throw new System.NotImplementedException();
    }

    
    protected override void OnRemovedEffects(Entity target)
    {
       if(SOType.isBuff)champ.DecreaseCurrentMoveSpeedRPC(boost);
       else
       {
           Debug.Log($"Unslow {champ.gameObject.name} with {SOType.name}");
           champ.IncreaseCurrentMoveSpeedRPC(boost);
       }
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
            OnRemoved();
        }

    }
}
