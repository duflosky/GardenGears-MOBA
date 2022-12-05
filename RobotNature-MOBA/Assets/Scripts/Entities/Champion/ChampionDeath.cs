using System.Collections;
using System.Collections.Generic;
using Entities.FogOfWar;
using GameStates;
using Photon.Pun;
using UnityEngine;

public partial class Champion
{
    public bool isAlive;
    public bool canDie;

    public float respawnDuration = 3;
    private double respawnTimer;

    private Vector3 respawnPos;

    public bool IsAlive()
    {
        return isAlive;
    }

    public bool CanDie()
    {
        return canDie;
    }

    public void RequestSetCanDie(bool value)
    {
        photonView.RPC("SetCanDieRPC", RpcTarget.MasterClient, value);
    }

    [PunRPC]
    public void SyncSetCanDieRPC(bool value)
    {
        canDie = value;
        OnSetCanDieFeedback?.Invoke(value);
    }

    [PunRPC]
    public void SetCanDieRPC(bool value)
    {
        canDie = value;
        OnSetCanDie?.Invoke(value);
        photonView.RPC("SyncSetCanDieRPC", RpcTarget.All, value);
    }

    public event GlobalDelegates.BoolDelegate OnSetCanDie;
    public event GlobalDelegates.BoolDelegate OnSetCanDieFeedback;

    public void RequestDie()
    {
        photonView.RPC("DieRPC", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void SyncDieRPC()
    {
        if (photonView.IsMine)
        {
            InputManager.PlayerMap.Movement.Disable();
            // InputManager.PlayerMap.Attack.Disable();
            InputManager.PlayerMap.Capacity.Disable();
            // InputManager.PlayerMap.Inventory.Disable();
        }

        rotateParent.gameObject.SetActive(false);
        TransformUI.gameObject.SetActive(false);
        FogOfWarManager.Instance.RemoveFOWViewable(this);

        OnDieFeedback?.Invoke();
    }

    [PunRPC]
    public void DieRPC()
    {
        // TODO : More useful to use that mechanic on decreaseCurrentHp ?
        if (!canDie)
        {
            Debug.LogWarning($"{name} can't die!");
            return;
        }
        isAlive = false;
        OnDie?.Invoke();
        GameStateMachine.Instance.OnTick += Revive;
        photonView.RPC("SyncDieRPC", RpcTarget.All);
    }

    public event GlobalDelegates.NoParameterDelegate OnDie;
    public event GlobalDelegates.NoParameterDelegate OnDieFeedback;

    public void RequestRevive()
    {
        photonView.RPC("ReviveRPC", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void SyncReviveRPC()
    {
        transform.position = respawnPos;
        if (photonView.IsMine)
        {
            InputManager.PlayerMap.Movement.Enable();
            // InputManager.PlayerMap.Attack.Enable();
            InputManager.PlayerMap.Capacity.Enable();
            // InputManager.PlayerMap.Inventory.Enable();
        }

        FogOfWarManager.Instance.AddFOWViewable(this);
        rotateParent.gameObject.SetActive(true);
        TransformUI.gameObject.SetActive(true);
        OnReviveFeedback?.Invoke();
    }

    [PunRPC]
    public void ReviveRPC()
    {
        isAlive = true;

        SetCurrentHpRPC(maxHp);
        SetCurrentResourceRPC(maxResource);
        OnRevive?.Invoke();
        photonView.RPC("SyncReviveRPC", RpcTarget.All);
    }

    private void Revive()
    {
        respawnTimer += 1 / GameStateMachine.Instance.tickRate;
        if (!(respawnTimer >= respawnDuration)) return;
        GameStateMachine.Instance.OnTick -= Revive;
        respawnTimer = 0f;
        RequestRevive();
    }

    public event GlobalDelegates.NoParameterDelegate OnRevive;
    public event GlobalDelegates.NoParameterDelegate OnReviveFeedback;

}
