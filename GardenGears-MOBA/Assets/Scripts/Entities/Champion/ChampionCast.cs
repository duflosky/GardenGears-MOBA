using Entities.Capacities;
using GameStates;
using Photon.Pun;
using UnityEngine;

public partial class Champion
{ 
    [Header("=== CAST")] 
    public byte[] abilitiesIndexes = new byte[2];
    public byte ultimateAbilityIndex;

    private double[] abilityCooldowns = new double[4];

    public bool canCast;
    public bool isCasting;
    
    public bool CanCast()
    {
        return canCast;
    }

    public void RequestSetCanCast(bool value)
    {
        photonView.RPC("SetCanCastRPC", RpcTarget.All, value);
    }

    [PunRPC]
    public void SetCanCastRPC(bool value)
    {
        canCast = value;
        OnSetCanCast?.Invoke(value);
    }

    public event GlobalDelegates.BoolDelegate OnSetCanCast;

    public void DecreaseCooldown()
    {
        for (int i = 0; i < abilityCooldowns.Length; i++)
        {
            if (abilityCooldowns[i] > 0)
            {
                abilityCooldowns[i]--;
            }
        }
    }
    
    public void RequestCast(byte capacityIndex, byte championCapacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        photonView.RPC("CastRPC", RpcTarget.All, capacityIndex, championCapacityIndex, targetedEntities, targetedPositions);
    }

    [PunRPC]
    public void CastRPC(byte capacityIndex, byte championCapacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        if (abilityCooldowns[championCapacityIndex] > 0 || isCasting) return;
        var activeCapacity = CapacitySOCollectionManager.CreateActiveCapacity(capacityIndex,this);
        if (!activeCapacity.TryCast(targetedEntities, targetedPositions)) return;
        abilityCooldowns[championCapacityIndex] = CapacitySOCollectionManager.GetActiveCapacitySOByIndex(capacityIndex).cooldown * GameStateMachine.Instance.tickRate;
        if (animator)
        {
            foreach (var animatorControllerParameter in animator.parameters)
            {
                if (!animatorControllerParameter.name.Contains(activeCapacity.SO.referenceName)) continue;
                animator.SetTrigger(animatorControllerParameter.name);
            }
        }
        else OnCastAnimationCast.Invoke(transform);
        OnCastFeedback?.Invoke(capacityIndex, targetedEntities, targetedPositions, activeCapacity);
    }
    
    public void CastAnimationCast(Transform transform)
    {
        OnCastAnimationCast?.Invoke(transform);
        int[] emptyIntArray = null;
        Vector3[] emptyVector3Array = null;
        OnCastAnimationCastFeedback?.Invoke(entityIndex, emptyIntArray, emptyVector3Array);
    }

    public void CastAnimationEnd()
    {
        OnCastAnimationEnd?.Invoke();
    }

    public void CastAnimationFeedback()
    {
        OnCastAnimationFeedback?.Invoke();
    }
    
    public void CastAnimationShotEffect(Transform transform)
    {
        OnCastAnimationShotEffect?.Invoke(transform);
    }
    
    public event GlobalDelegates.NoParameterDelegate CastUpdate;
    public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnCast;
    public event GlobalDelegates.TransformDelegate OnCastAnimationCast;
    public event GlobalDelegates.NoParameterDelegate OnCastAnimationEnd;
    public event GlobalDelegates.NoParameterDelegate OnCastAnimationFeedback;
    public event GlobalDelegates.TransformDelegate OnCastAnimationShotEffect;
    public event GlobalDelegates.IntIntArrayVector3ArrayDelegate OnCastAnimationCastFeedback;
    public event GlobalDelegates.ByteIntArrayVector3ArrayCapacityDelegate OnCastFeedback;
}