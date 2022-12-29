using System;
using GameStates;
using UnityEngine;

namespace Entities.Minion
{
    public class MinionController : Controllers.Controller
    {
        public enum MinionState { Idle, Walking, LookingForPathing, Attacking }
        public MinionState currentState = MinionState.Idle;
        private float timer;
        private Minion myMinion;
    
        private void OnEnable()
        {
            myMinion = controlledEntity.GetComponent<Minion>();
            currentState = MinionState.LookingForPathing;
            GameStateMachine.Instance.OnTick += AiLogic;
        }

        private void AiLogic()
        {
            timer += Time.deltaTime;
            if (timer >= GameStateMachine.Instance.tickRate) return;
            timer = 0;
            switch (currentState)
            {
                case MinionState.Idle: myMinion.IdleState(); break;
                case MinionState.Walking: myMinion.WalkingState(); break;
                case MinionState.LookingForPathing: myMinion.LookingForPathingState(); break;
                case MinionState.Attacking: myMinion.AttackingState(); break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}