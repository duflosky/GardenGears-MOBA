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
    public ChampionActiveCapacitySO ChampSO;
    private GameObject gizmo;
    
    
    public override void OnStart()
    {
        ChampSO = (ChampionActiveCapacitySO)SO;
        casterTransform = caster.transform;
        champion = (Champion)caster;
    }
    
    public override void CapacityPress()
    {
        if (ChampSO.capacitySlow == null) throw new NullReferenceException($"Missing capacitySlow to {ChampSO.name}");
        champion.GetPassiveCapacity(CapacitySOCollectionManager.GetPassiveCapacitySOIndex(ChampSO.capacitySlow)).OnAdded();
        DisplayGizmos(true);
        champion.OnCastAnimationCast += CapacityEffect;
        champion.OnCastAnimationEnd += CapacityEndAnimation;
        champion.OnCastAnimationShotEffect += CapacityShotEffect;
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
            if (ChampSO.gizmoPrefab == null) 
            {
                //throw new NullReferenceException($"Missing Gizmo Prefab To {ChampSO.name}");
                Debug.LogWarning($"Missing Gizmo Prefab To {ChampSO.name}");
                return;
            }
            if (!gizmo)
            {
                gizmo = Object.Instantiate(ChampSO.gizmoPrefab, casterTransform.position, Quaternion.identity, casterTransform);
                var rect = gizmo.GetComponentInChildren<Image>().GetComponent<RectTransform>(); //Désolé pour les yeux :3
                rect.localPosition =(new Vector3(0,0,ChampSO.maxRange/2));
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, ChampSO.maxRange);
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
        var pos = targetPositions[0];
        pos.y = gizmo.transform.position.y;
        gizmo.transform.LookAt(pos);
    }
    
    
}
