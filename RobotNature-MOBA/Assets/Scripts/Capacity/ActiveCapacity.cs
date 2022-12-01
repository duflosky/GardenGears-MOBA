using System.Collections;
using System.Collections.Generic;
using GameStates;
using UnityEngine;

namespace Entities.Capacities
{
    public abstract class ActiveCapacity
    {
        public byte indexOfSOInCollection;
        public ActiveCapacitySO SO;
        public Entity caster;
        
        private double cooldownTimer;
        public bool onCooldown;
        
        protected ActiveCapacitySO AssociatedActiveCapacitySO()
        {
            return CapacitySOCollectionManager.GetActiveCapacitySOByIndex(indexOfSOInCollection);
        }

        #region Cast

        protected virtual void InitiateCooldown()
        {

            cooldownTimer = SO.cooldown;
            onCooldown = true;


            GameStateMachine.Instance.OnTick += CooldownTimer;
        }

        /// <summary>
        /// Method which update the timer.
        /// </summary>
        protected virtual void CooldownTimer()
        {
            cooldownTimer -= 1.0 / GameStateMachine.Instance.tickRate;

            if (cooldownTimer <= 0)
            {
                onCooldown = false;
                Debug.Log(onCooldown);
                GameStateMachine.Instance.OnTick -= CooldownTimer;
            }
        }

        public virtual bool TryCast(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            // if (Vector3.Distance(EntityCollectionManager.GetEntityByIndex(casterIndex).transform.position, EntityCollectionManager.GetEntityByIndex(targetsEntityIndexes[0]).transform.position)> 
            //     AssociatedActiveCapacitySO().maxRange) return false;

            if (!onCooldown)
            {
                InitiateCooldown();
                return true;
            }
            else return false;
        }

        public virtual void CollideEffect(Entity entityAffect)
        {
            
        }

        public virtual bool isInRange(int casterIndex, Vector3 position)
        {
            float distance = Vector3.Distance(EntityCollectionManager.GetEntityByIndex(casterIndex).transform.position,
                position);
            //Debug.Log($"distance:{distance}  >  range:{ AssociatedActiveCapacitySO().maxRange}");
            if (distance > SO.maxRange) return false;

            return true;
        }

        #endregion

        #region MyRegion

        public abstract void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions);

        #endregion
    }
}
