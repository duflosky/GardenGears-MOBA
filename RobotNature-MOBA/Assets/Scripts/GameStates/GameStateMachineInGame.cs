using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GameStates
{
    public partial class GameStateMachine
    {
        [Header("In Game")] 
        [SerializeField] private GameObject timerPrefab;

        public GameTimer InitGameTimer()
        {
            if (!PhotonNetwork.IsMasterClient) return null;
            return PhotonNetwork.Instantiate(timerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<GameTimer>();
        }
        
        [PunRPC]
        private void SyncWinnerRPC(byte team)
        {
            winner = (Enums.Team)team;
        }
        
    }

}