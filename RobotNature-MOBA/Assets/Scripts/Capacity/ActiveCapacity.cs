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
        public Transform casterTransform;

        protected int[] targetsEntityIndexes;
        protected Vector3[] targetPositions;
        
        private double cooldownTimer;
        public bool onCooldown;
        
        protected ActiveCapacitySO AssociatedActiveCapacitySO()
        {
            return CapacitySOCollectionManager.GetActiveCapacitySOByIndex(indexOfSOInCollection);
        }

        public abstract void OnStart();

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
                GameStateMachine.Instance.OnTick -= CooldownTimer;
            }
        }

        public virtual bool TryCast( int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            // if (Vector3.Distance(EntityCollectionManager.GetEntityByIndex(casterIndex).transform.position, EntityCollectionManager.GetEntityByIndex(targetsEntityIndexes[0]).transform.position)> 
            //     AssociatedActiveCapacitySO().maxRange) return false;

            if (!onCooldown)
            {
                InitiateCooldown();
                this.targetsEntityIndexes = targetsEntityIndexes;

                this.targetPositions = new Vector3[targetPositions.Length];
                for (int i = 0; i < targetPositions.Length; i++)
                {
                    this.targetPositions[i] = targetPositions[i];
                }
                CapacityPress();
                return true;
            }
            else return false;
        }

        public abstract void CapacityPress();
        public abstract void CapacityEffect(Transform transform);
        public virtual void CapacityEndAnimation(){}
        public virtual void CapacityRelease(){}

        public virtual void CollideEntityEffect(Entity entityAffect)
        {
            
        }

        public virtual void CollideObjectEffect(GameObject obj)
        {
            Debug.Log("Collide Obejct");
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
