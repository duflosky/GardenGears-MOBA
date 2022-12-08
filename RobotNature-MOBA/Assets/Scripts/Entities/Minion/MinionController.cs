using System;
using UnityEngine;

namespace Entities.Minion
{
    public class MinionController : Controllers.Controller
    {
        public enum MinionState { Idle, Walking, LookingForPathing, Attacking }
        public MinionState currentState = MinionState.Idle;
        public float brainSpeed = .7f;
        private float brainTimer;
        private Minion myMinion;
    
        private void OnEnable()
        {
            myMinion = controlledEntity.GetComponent<Minion>();
            currentState = MinionState.LookingForPathing;
        }
    
        void Update()
        {
            // Créer des ticks pour éviter le saut de frame
            brainTimer += Time.deltaTime;
            if (brainTimer >= brainSpeed)
            {
                AiLogic();
                brainTimer = 0;
            }
        }

        private void AiLogic()
        {
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
