using System.Linq;
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
    
    public bool CanCast()
    {
        return canCast;
    }

    public void RequestSetCanCast(bool value)
    {
        photonView.RPC("SetCanCastRPC", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void SetCanCastRPC(bool value)
    {
        canCast = value;
        OnSetCanCast?.Invoke(value);
        photonView.RPC("SyncCastRPC", RpcTarget.All, canCast);
    }

    [PunRPC]
    public void SyncSetCanCastRPC(bool value)
    {
        canCast = value;
        OnSetCanCastFeedback?.Invoke(value);
    }

    public event GlobalDelegates.BoolDelegate OnSetCanCast;
    public event GlobalDelegates.BoolDelegate OnSetCanCastFeedback;

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
        if (abilityCooldowns[championCapacityIndex] > 0) return;
        photonView.RPC("CastRPC",RpcTarget.MasterClient,capacityIndex, championCapacityIndex,targetedEntities,targetedPositions);
    }

    [PunRPC]
    public void CastRPC(byte capacityIndex, byte championCapacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        if (abilityCooldowns[championCapacityIndex] > 0) return;
        var activeCapacity = CapacitySOCollectionManager.CreateActiveCapacity(capacityIndex,this);
        if (!activeCapacity.TryCast(targetedEntities, targetedPositions)) return;
        abilityCooldowns[championCapacityIndex] = CapacitySOCollectionManager.GetActiveCapacitySOByIndex(capacityIndex).cooldown*GameStateMachine.Instance.tickRate;
        OnCast?.Invoke(capacityIndex, targetedEntities, targetedPositions);
        photonView.RPC("SyncCastRPC", RpcTarget.All, capacityIndex, targetedEntities, targetedPositions);
    }
    

    [PunRPC]
    public void SyncCastRPC(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        var activeCapacity = CapacitySOCollectionManager.CreateActiveCapacity(capacityIndex, this);
        activeCapacity.PlayFeedback(capacityIndex, targetedEntities, targetedPositions);
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
    }

    public void CastAnimationEnd()
    {
        OnCastAnimationEnd?.Invoke();
    }
    
    public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnCast;
    public event GlobalDelegates.TransformDelegate OnCastAnimationCast;
    public event GlobalDelegates.NoParameterDelegate OnCastAnimationEnd;
    public event GlobalDelegates.ByteIntArrayVector3ArrayCapacityDelegate OnCastFeedback;
}