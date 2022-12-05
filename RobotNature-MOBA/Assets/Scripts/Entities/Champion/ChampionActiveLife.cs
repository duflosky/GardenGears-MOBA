using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public partial class Champion
{
     [SerializeField] private bool attackAffect = true;
    [SerializeField] private bool abilitiesAffect = true;
    private float maxHp;
    private float currentHp;

    public float GetMaxHp()
    {
        return maxHp;
    }

    public float GetCurrentHp()
    {
        return currentHp;
    }

    public bool AttackAffected()
    {
        return attackAffect;
    }

    public bool AbilitiesAffected()
    {
        return abilitiesAffect;
    }

    public void RequestSetMaxHp(float value)
    {
        photonView.RPC("SetMaxHpRPC", RpcTarget.MasterClient, value);
    }

    [PunRPC]
    public void SyncSetMaxHpRPC(float value)
    {
        maxHp = value;
        currentHp = value;
        OnSetMaxHpFeedback?.Invoke(value);
    }

    [PunRPC]
    public void SetMaxHpRPC(float value)
    {
        maxHp = value;
        currentHp = value;
        OnSetMaxHp?.Invoke(value);
        photonView.RPC("SyncSetMaxHpRPC", RpcTarget.All, maxHp);
    }

    public event GlobalDelegates.FloatDelegate OnSetMaxHp;
    public event GlobalDelegates.FloatDelegate OnSetMaxHpFeedback;

    public void RequestIncreaseMaxHp(float amount)
    {
        photonView.RPC("IncreaseMaxHpRPC", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    public void SyncIncreaseMaxHpRPC(float amount)
    {
        maxHp = amount;
        currentHp = amount;
        OnIncreaseMaxHpFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void IncreaseMaxHpRPC(float amount)
    {
        maxHp += amount;
        currentHp = amount;
        if (maxHp < currentHp) currentHp = maxHp;
        OnIncreaseMaxHp?.Invoke(amount);
        photonView.RPC("SyncIncreaseMaxHpRPC", RpcTarget.All, maxHp);
    }

    public event GlobalDelegates.FloatDelegate OnIncreaseMaxHp;
    public event GlobalDelegates.FloatDelegate OnIncreaseMaxHpFeedback;

    public void RequestDecreaseMaxHp(float amount)
    {
        photonView.RPC("DecreaseMaxHpRPC", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    public void SyncDecreaseMaxHpRPC(float amount)
    {
        maxHp = amount;
        if (maxHp < currentHp) currentHp = maxHp;
        OnDecreaseMaxHpFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void DecreaseMaxHpRPC(float amount)
    {
        maxHp -= amount;
        if (maxHp < currentHp) currentHp = maxHp;
        OnDecreaseMaxHp?.Invoke(amount);
        photonView.RPC("SyncDecreaseMaxHpRPC", RpcTarget.All, maxHp);
    }

    public event GlobalDelegates.FloatDelegate OnDecreaseMaxHp;
    public event GlobalDelegates.FloatDelegate OnDecreaseMaxHpFeedback;

    public void RequestSetCurrentHp(float value)
    {
        photonView.RPC("SetCurrentHpRPC", RpcTarget.MasterClient, value);
    }

    [PunRPC]
    public void SyncSetCurrentHpRPC(float value)
    {
        currentHp = value;
        OnSetCurrentHpFeedback?.Invoke(value);
    }

    [PunRPC]
    public void SetCurrentHpRPC(float value)
    {
        currentHp = value;
        OnSetCurrentHp?.Invoke(value);
        photonView.RPC("SyncSetCurrentHpRPC", RpcTarget.All, value);
    }

    public event GlobalDelegates.FloatDelegate OnSetCurrentHp;
    public event GlobalDelegates.FloatDelegate OnSetCurrentHpFeedback;

    public void RequestIncreaseCurrentHp(float amount)
    {
        photonView.RPC("IncreaseCurrentHpRPC", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    public void SyncIncreaseCurrentHpRPC(float amount)
    {
        currentHp = amount;
        OnIncreaseCurrentHpFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void IncreaseCurrentHpRPC(float amount)
    {
        currentHp += amount;
        if (currentHp > maxHp) currentHp = maxHp;
        OnIncreaseCurrentHp?.Invoke(amount);
        photonView.RPC("SyncIncreaseCurrentHpRPC", RpcTarget.All, currentHp);
    }

    public event GlobalDelegates.FloatDelegate OnIncreaseCurrentHp;
    public event GlobalDelegates.FloatDelegate OnIncreaseCurrentHpFeedback;

    public void RequestDecreaseCurrentHp(float amount)
    {
        photonView.RPC("SyncDecreaseCurrentHpRPC", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    public void DecreaseCurrentHpRPC(float amount)
    {
        currentHp = amount;
        if (currentHp <= 0)
        {
            currentHp = 0;
            RequestDie();
        }
        OnDecreaseCurrentHpFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void SyncDecreaseCurrentHpRPC(float amount)
    {
        currentHp -= amount;
        OnDecreaseCurrentHp?.Invoke(amount);
        photonView.RPC("DecreaseCurrentHpRPC", RpcTarget.All, currentHp);
    }


    public event GlobalDelegates.FloatDelegate OnDecreaseCurrentHp;
    public event GlobalDelegates.FloatDelegate OnDecreaseCurrentHpFeedback;

}
