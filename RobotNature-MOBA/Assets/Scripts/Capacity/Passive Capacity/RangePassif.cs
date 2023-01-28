using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Capacities;
using GameStates;
using UnityEngine;


public class RangePassif : PassiveCapacity
{
    private RangePassifSO SOType;
    private Champion champ;
    private SpeedModifierPassive speedBoost;

    private int burnTimer;
    private int actifTimer;
    
    private int stackTimer;
    
    private bool haveStack;

    public override void OnCreate()
    {
        SOType = (RangePassifSO)SO;
        champ = (Champion)entity;
        champ.SetMaxResourceRPC(SOType.maxHeatStack);
        speedBoost = (SpeedModifierPassive)champ.GetPassiveCapacity(CapacitySOCollectionManager.GetPassiveCapacitySOIndex(SOType.actifSpeedBoost));
    }

    protected override void OnAddedEffects(Entity target)
    {
        stackTimer = 0;
        if (count > SOType.maxHeatStack)
        {
            FullStackProc();
        }
        else
        {
            champ.IncreaseCurrentResourceRPC(1);
            if (!haveStack)
            {
                GameStateMachine.Instance.OnTick += DecreaseStack;
                haveStack = true;
            }
        }
    }

    protected override void OnAddedFeedbackEffects(Entity target)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnRemovedEffects(Entity target)
    {
        if (count < SOType.overheatStack && champ.isOverheat)
        {
            champ.isOverheat = false;
            GameStateMachine.Instance.OnTick -= BurnFeedback;
        }
    }

    protected override void OnRemovedFeedbackEffects(Entity target)
    {
        throw new System.NotImplementedException();
    }

    //===================================================================================

    void FullStackProc()
    {
        speedBoost.OnAdded();
        count = 0;
        haveStack = false;
        champ.isOverheat = true;
        champ.SetCurrentResourceRPC(0);
        GameStateMachine.Instance.OnTick -= DecreaseStack;
        GameStateMachine.Instance.OnTick += ActifTimer;
        GameStateMachine.Instance.OnTick += BurnFeedback;
    }
    
    
    void BurnFeedback()
    {
        burnTimer++;
        if (burnTimer >= SOType.BurnDelay * GameStateMachine.Instance.tickRate)
        {
            burnTimer = 0;
            //Burn
        }
    }

    void DecreaseStack()
    {
        stackTimer++;
        if (stackTimer >= SOType.stackDuration * GameStateMachine.Instance.tickRate)
        {
            count--;
            champ.DecreaseCurrentResourceRPC(1);
            stackTimer = 0;
            if (count <= 0)
            {
                count = 0;
                GameStateMachine.Instance.OnTick -= DecreaseStack;
                haveStack = false;
            }
        }
    }

    void ActifTimer()
    {
        actifTimer++;
        if (actifTimer >= SOType.actifDuration*GameStateMachine.Instance.tickRate)
        {
            actifTimer = 0;
            champ.isOverheat = false;
            speedBoost.OnRemoved();
            GameStateMachine.Instance.OnTick -= ActifTimer;
            GameStateMachine.Instance.OnTick -= BurnFeedback;


        }
    }
}
