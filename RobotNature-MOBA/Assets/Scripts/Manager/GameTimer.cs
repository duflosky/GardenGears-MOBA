using System.Collections;
using System.Collections.Generic;
using GameStates;
using GameStates.States;
using Photon.Pun;
using UI.InGame;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class GameTimer : MonoBehaviourPun
{
    [SerializeField] private int minute = 5;
    [SerializeField] private int second;

    public InGameState sm;
    
    public void StartTimer()
    {
        GameStateMachine.Instance.OnSecondTick += UpdateTime;
    }

    void UpdateTime()
    {
        if (second == 0)
        {
            if (minute == 0) EndTimer();
            else
            {
                minute--;
                second = 59;
            }
        }
        else second--;
        photonView.RPC("SyncTimerRPC", RpcTarget.All, minute, second);
    }

    [PunRPC]
    public void SyncTimerRPC(int minute, int second)
    {
        UIManager.Instance.UpdateTimerText(minute, second);
    }
    
    void EndTimer()
    {
        sm.SetWinner();
        GameStateMachine.Instance.OnSecondTick -= UpdateTime;
    }
}
