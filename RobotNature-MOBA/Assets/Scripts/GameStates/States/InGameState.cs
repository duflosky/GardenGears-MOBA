using Photon.Pun;
using UnityEngine;

namespace GameStates.States
{
    public class InGameState : GameState
    {
        public InGameState(GameStateMachine sm) : base(sm) { }

        private double timer;
        private double lastTickTime;

        public override void StartState()
        {
            InputManager.EnablePlayerMap(true);
            lastTickTime = PhotonNetwork.Time;
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
            
            if (timer >= 1.0 / sm.tickRate)
            {
                sm.Tick();
                lastTickTime = PhotonNetwork.Time;
            }
        }

        public override void ExitState() { }

        public override void OnAllPlayerReady() { }

        private bool IsWinConditionChecked()
        {
            // Check win condition for any team
            return sm.winner != Enums.Team.Neutral;
        }
    }
}