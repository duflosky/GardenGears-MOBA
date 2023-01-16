using System;
using Photon.Pun;
using UI.InGame;
using UnityEngine;

namespace GameStates.States
{
    public class InGameState : GameState
    {
        public InGameState(GameStateMachine sm) : base(sm) { }

        private double timer;
        private double lastTickTime;
        
        private double secondTimer;
        private double lastSecondTime;
        private bool overTime;

        private int team1Kill;
        private int team2Kill;
        

        public override void StartState()
        {
            InputManager.EnablePlayerMap(true);
            lastTickTime = lastSecondTime = PhotonNetwork.Time;
            if (!PhotonNetwork.IsMasterClient) return;
            var timer = sm.InitGameTimer();
            timer.sm = this;
            timer.StartTimer();
        }

        public override void UpdateState()
        {
            if (IsWinConditionChecked())
            {
                Debug.Log("Boolean condition true");
                sm.SendWinner(sm.winner);
                sm.SwitchState(3);
                return;
            }

            timer = PhotonNetwork.Time - lastTickTime;
            secondTimer = PhotonNetwork.Time - lastSecondTime;
            
            if (timer >= 1.0 / sm.tickRate)
            {
                sm.Tick();
                lastTickTime = PhotonNetwork.Time;
            }

            if (secondTimer >= 1.0)
            {
                sm.SecondTick();
                lastSecondTime = PhotonNetwork.Time;
            }
        }

        public override void ExitState() { }

        public override void OnAllPlayerReady() { }

        public void AddPoint(Enums.Team pointTeam)
        {
            if (pointTeam == Enums.Team.Team1) team1Kill++;
            else team2Kill++;
            if(overTime) SetWinner();
            sm.photonView.RPC("SyncTeamKillRPC", RpcTarget.All, team1Kill, team2Kill);
        }

        public void SetWinner()
        {
            if (team1Kill > team2Kill) sm.winner = Enums.Team.Team1;
            else if (team1Kill < team2Kill) sm.winner = Enums.Team.Team2;
            else overTime = true;
        }
        
        private bool IsWinConditionChecked()
        {
            // Check win condition for any team
            return sm.winner != Enums.Team.Neutral;
        }
    }
}