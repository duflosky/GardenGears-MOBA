using System;
using Entities.Capacities;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public abstract class  ChampionActiveCapacity : ActiveCapacity
{
    public ChampionActiveCapacitySO ChampSO;
    protected Champion champion;
    private GameObject gizmo;

    public override void OnStart()
    {
        ChampSO = (ChampionActiveCapacitySO)SO;
        casterTransform = caster.transform;
        champion = (Champion)caster;
    }
    
    public override void CapacityPress()
    {
        if (ChampSO.capacitySlow == null) throw new NullReferenceException($"Missing capacitySlow on {ChampSO.name}");
        champion.GetPassiveCapacity(ChampSO.capacitySlow).OnAdded();
        champion.OnCastAnimationShotEffect += CapacityShotEffect;
        champion.OnCastAnimationCastFeedback += PlayFeedback;
        champion.OnCastAnimationEnd += CapacityEndAnimation;
    }
    
    public override bool TryCast(int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (onCooldown) return false;
        InitiateCooldown();
        CapacityPress();
        return true;
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (champion.photonView.IsMine) DisplayGizmos(true);
    }

    private void DisplayGizmos(bool state)
    {
        if (state)
        {
            if (ChampSO.gizmoPrefab == null) 
            {
                Debug.LogWarning($"Missing Gizmo Prefab on {ChampSO.name}");
                return;
            }
            if (!gizmo)
            {
                gizmo = Object.Instantiate(ChampSO.gizmoPrefab, casterTransform.position, Quaternion.identity, casterTransform);
                var rect = gizmo.GetComponentInChildren<Image>().GetComponent<RectTransform>();
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
                Debug.LogError($"Gizmo is not assign;");
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

    private void UpdateGizmos()
    {
        gizmo.transform.rotation = casterTransform.GetChild(0).rotation;
    }
}
