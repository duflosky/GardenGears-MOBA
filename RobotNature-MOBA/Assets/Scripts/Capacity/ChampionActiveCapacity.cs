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
        champion.OnCastAnimationCast += CapacityEffect;
        champion.OnCastAnimationEnd += CapacityEndAnimation;
        champion.OnCastAnimationShotEffect += CapacityShotEffect;
    }
    
    public override bool TryCast(int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (!onCooldown)
        {
            InitiateCooldown();

            CapacityPress();
            return true;
        }
        else return false;
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        Debug.Log("Play FeedBack");
        if(champion.photonView.IsMine)DisplayGizmos(true);
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
                Debug.Log($"Create Gizmo: {gizmo}");
                var rect = gizmo.GetComponentInChildren<Image>().GetComponent<RectTransform>(); //Désolé pour les yeux :3
                rect.localPosition =(new Vector3(0,0,ChampSO.maxRange/2));
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, ChampSO.maxRange);
            }
            else gizmo.SetActive(true);
            champion.CastUpdate += UpdateGizmos;
            champion.OnCastAnimationCast += DisableGizmo;
        }
        else
        {
            if (!gizmo)
            {
                Debug.LogError($"Gizmo Not Assign");
                return;
            }
            champion.CastUpdate -= UpdateGizmos;
            gizmo.SetActive(false);
        }
    }

    private void DisableGizmo(Transform t)
    {
        DisplayGizmos(false);
    }

    public virtual void UpdateGizmos()
    {
        gizmo.transform.rotation = casterTransform.GetChild(0).rotation;
    }
    
    
}
