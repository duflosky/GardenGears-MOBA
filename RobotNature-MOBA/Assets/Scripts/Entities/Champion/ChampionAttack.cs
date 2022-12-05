using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public partial class Champion
{
    public bool CanAttack()
    {
        throw new System.NotImplementedException();
    }

    public void RequestSetCanAttack(bool value)
    {
        photonView.RPC("SetCanAttackRPC", RpcTarget.MasterClient, value);
    }

    public void SyncSetCanAttackRPC(bool value)
    {
        OnSetCanAttackFeedback?.Invoke(value);
    }

    public void SetCanAttackRPC(bool value)
    {
        OnSetCanAttack?.Invoke(value);
        photonView.RPC("SyncSetCanAttackRPC", RpcTarget.All, value);
    }

    public event GlobalDelegates.BoolDelegate OnSetCanAttack;
    public event GlobalDelegates.BoolDelegate OnSetCanAttackFeedback;
    public float GetAttackDamage()
    {
        throw new System.NotImplementedException();
    }

    public void RequestSetAttackDamage(float value)
    {
        photonView.RPC("SetAttackDamageRPC", RpcTarget.MasterClient, value);
    }

    public void SyncSetAttackDamageRPC(float value)
    {
        OnSetAttackDamageFeedback?.Invoke(value);
    }

    public void SetAttackDamageRPC(float value)
    {
        OnSetAttackDamage?.Invoke(value);
        photonView.RPC("SyncSetAttackDamageRPC", RpcTarget.All, value);
    }

    public event GlobalDelegates.FloatDelegate OnSetAttackDamage;
    public event GlobalDelegates.FloatDelegate OnSetAttackDamageFeedback;
    public void RequestAttack(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        photonView.RPC("AttackRPC", RpcTarget.MasterClient, capacityIndex, targetedEntities, targetedPositions);
    }

    public void SyncAttackRPC(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        OnAttackFeedback?.Invoke(capacityIndex, targetedEntities, targetedPositions);
    }

    public void AttackRPC(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        OnAttack?.Invoke(capacityIndex, targetedEntities, targetedPositions);
        photonView.RPC("SyncAttackRPC", RpcTarget.All, capacityIndex, targetedEntities, targetedPositions);
    }

    public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnAttack;
    public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnAttackFeedback;
    public void RequestIncreaseAttackDamage(float value)
    {
        photonView.RPC("IncreaseAttackDamageRPC", RpcTarget.MasterClient, value);
    }

    public void SyncIncreaseAttackDamageRPC(float value)
    {
        OnIncreaseAttackDamageFeedback?.Invoke(value);
    }

    public void IncreaseAttackDamageRPC(float value)
    {
        OnIncreaseAttackDamage?.Invoke(value);
        photonView.RPC("SyncIncreaseAttackDamageRPC", RpcTarget.All, value);
    }

    public event GlobalDelegates.FloatDelegate OnIncreaseAttackDamage;
    public event GlobalDelegates.FloatDelegate OnIncreaseAttackDamageFeedback;
    public void RequestDecreaseAttackDamage(float value)
    {
        photonView.RPC("DecreaseAttackDamageRPC", RpcTarget.MasterClient, value);
    }

    public void SyncDecreaseAttackDamageRPC(float value)
    {
        OnDecreaseAttackDamageFeedback?.Invoke(value);
    }

    public void DecreaseAttackDamageRPC(float value)
    {
        OnDecreaseAttackDamage?.Invoke(value);
        photonView.RPC("SyncDecreaseAttackDamageRPC", RpcTarget.All, value);
    }

    public event GlobalDelegates.FloatDelegate OnDecreaseAttackDamage;
    public event GlobalDelegates.FloatDelegate OnDecreaseAttackDamageFeedback;
    public void RequestIncreaseAttackSpeed(float value)
    {
        photonView.RPC("IncreaseAttackSpeedRPC", RpcTarget.MasterClient, value);
    }

    public void SyncIncreaseAttackSpeedRPC(float value)
    {
        OnIncreaseAttackSpeedFeedback?.Invoke(value);
    }

    public void IncreaseAttackSpeedRPC(float value)
    {
        OnIncreaseAttackSpeed?.Invoke(value);
        photonView.RPC("SyncIncreaseAttackSpeedRPC", RpcTarget.All, value);
    }

    public event GlobalDelegates.FloatDelegate OnIncreaseAttackSpeed;
    public event GlobalDelegates.FloatDelegate OnIncreaseAttackSpeedFeedback;
    public void RequestDecreaseAttackSpeed(float value)
    {
        photonView.RPC("DecreaseAttackSpeedRPC", RpcTarget.MasterClient, value);
    }

    public void SyncDecreaseAttackSpeedRPC(float value)
    {
        OnDecreaseAttackSpeedFeedback?.Invoke(value);
    }

    public void DecreaseAttackSpeedRPC(float value)
    {
        OnDecreaseAttackSpeed?.Invoke(value);
        photonView.RPC("SyncDecreaseAttackSpeedRPC", RpcTarget.All, value);
    }

    public event GlobalDelegates.FloatDelegate OnDecreaseAttackSpeed;
    public event GlobalDelegates.FloatDelegate OnDecreaseAttackSpeedFeedback;
    
    public  GlobalDelegates.EntityDelegate OnDealDamage;

}
