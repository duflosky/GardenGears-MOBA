using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public abstract class ChampionActiveCapacity : ActiveCapacity
{
    protected Champion champion;
    public AutoAttackRangeSO SOType;
    private GameObject gizmo;
    public override void OnStart()
    {
        SOType = (AutoAttackRangeSO)SO;
        casterTransform = caster.transform;
        champion = (Champion)caster;
    }
    
    public override bool TryCast(int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (!onCooldown)
        {
            InitiateCooldown();
            this.targetsEntityIndexes = targetsEntityIndexes;

            this.targetPositions = targetPositions;
            CapacityPress();
            return true;
        }
        else return false;
    }

    public virtual void DisplayGizmos(bool state)
    {
        
        if (state)
        {
            if (SOType.gizmoPrefab == null) throw new NullReferenceException($"Missing Gizmo Prefab To {SOType.name}");
            if (!gizmo)
            {
                gizmo = Object.Instantiate(SOType.gizmoPrefab, casterTransform.position, Quaternion.identity, casterTransform);
                var rect = gizmo.GetComponentInChildren<Image>().GetComponent<RectTransform>(); //Désolé pour les yeux :3
                rect.localPosition =(new Vector3(0,0,SOType.maxRange/2));
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, SOType.maxRange);
            }
            else gizmo.SetActive(true);
            champion.CastUpdate += UpdateGizmos;
        }
        else
        {
            champion.CastUpdate -= UpdateGizmos;
            gizmo.SetActive(false);
        }
    }
    
    public virtual void UpdateGizmos()
    {
        gizmo.transform.LookAt(targetPositions[0]);
    }
    
    public override void CapacityPress()
    {
        champion.GetPassiveCapacity(CapacitySOCollectionManager.GetPassiveCapacitySOIndex(SOType.attackSlowSO)).OnAdded();
        DisplayGizmos(true);
        champion.OnCastAnimationCast += CapacityEffect;
        champion.OnCastAnimationEnd += CapacityEndAnimation;
    }
}
