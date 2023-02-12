using System;
using Entities.Capacities;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public abstract class ChampionActiveCapacity : ActiveCapacity
{
    protected Champion Champion;

    private ChampionActiveCapacitySO _championActiveCapacitySO;
    private GameObject _gizmo;

    public override void OnStart()
    {
        _championActiveCapacitySO = (ChampionActiveCapacitySO)ActiveCapacitySO;
        CasterTransform = Caster.transform;
        Champion = (Champion)Caster;
    }

    public override void CapacityPress()
    {
        if (_championActiveCapacitySO.CapacitySlowSO == null) throw new NullReferenceException($"Missing capacitySlow on {_championActiveCapacitySO.name}");
        Champion.GetPassiveCapacity(_championActiveCapacitySO.CapacitySlowSO).OnAdded();
        Champion.OnCastAnimationShotEffect += CapacityShotEffect;
        Champion.OnCastAnimationCastFeedback += PlayFeedback;
        Champion.OnCastAnimationEnd += CapacityEndAnimation;
    }

    public override bool TryCast(int[] targetsEntityIndexes, Vector3[] targetsPositions)
    {
        if (OnCooldown) return false;
        InitiateCooldown();
        CapacityPress();
        return true;
    }

    public override void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
    {
        if (Champion.photonView.IsMine) DisplayGizmos(true);
    }

    private void DisplayGizmos(bool state)
    {
        if (state)
        {
            if (_championActiveCapacitySO.Gizmo == null)
            {
                Debug.LogWarning($"Missing Gizmo Prefab on {_championActiveCapacitySO.name}");
                return;
            }

            if (!_gizmo)
            {
                _gizmo = Object.Instantiate(_championActiveCapacitySO.Gizmo, CasterTransform.position,
                    Quaternion.identity, CasterTransform);
                var rect = _gizmo.GetComponentInChildren<Image>().GetComponent<RectTransform>();
                rect.localPosition = (new Vector3(0, 0, _championActiveCapacitySO.maxRange / 2));
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, _championActiveCapacitySO.maxRange);
            }
            else _gizmo.SetActive(true);

            Champion.CastUpdate += UpdateGizmos;
            Champion.OnCastAnimationCast += DisableGizmo;
        }
        else
        {
            if (!_gizmo)
            {
                Debug.LogError($"Gizmo is not assign;");
                return;
            }

            Champion.CastUpdate -= UpdateGizmos;
            _gizmo.SetActive(false);
        }
    }

    private void DisableGizmo(Transform t)
    {
        DisplayGizmos(false);
    }

    private void UpdateGizmos()
    {
        _gizmo.transform.rotation = CasterTransform.GetChild(0).rotation;
    }
}