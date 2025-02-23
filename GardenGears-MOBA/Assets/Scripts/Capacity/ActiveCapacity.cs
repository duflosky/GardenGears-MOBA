using GameStates;
using UnityEngine;

namespace Entities.Capacities
{
    public abstract class ActiveCapacity
    {
        public ActiveCapacitySO SO;
        public bool onCooldown;
        public byte indexOfSOInCollection;
        public Entity caster;
        public Transform casterTransform;

        protected int[] targetsEntityIndexes;
        protected Vector3[] targetPositions;

        private double cooldownTimer;

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
            if (!(cooldownTimer <= 0)) return;
            onCooldown = false;
            GameStateMachine.Instance.OnTick -= CooldownTimer;
        }

        public virtual bool TryCast(int[] targetsEntityIndexes, Vector3[] targetPositions)
        {
            if (onCooldown) return false;
            InitiateCooldown();
            this.targetsEntityIndexes = targetsEntityIndexes;
            this.targetPositions = targetPositions;
            CapacityPress();
            return true;
        }

        public abstract void CapacityPress();
        
        public virtual void CapacityShotEffect(Transform transform) { }
        
        public abstract void CapacityEffect(Transform transform);
        
        public virtual void CapacityEndAnimation() { }
        
        public virtual void CapacityRelease() { }

        public virtual void CollideEntityEffect(Entity entity) { }

        public virtual void CollideObjectEffect(GameObject obj) { }
        
        public virtual void CollideFeedbackEffect(Entity affectedEntity) { }

        public virtual void CollideExitEffect(GameObject obj) { }
        
        public static event GlobalDelegates.ByteDelegate OnAllyHit;

        protected static void AllyHit(byte capacityIndex)
        {
            OnAllyHit?.Invoke(capacityIndex);
        }
        
        public virtual bool isInRange(int casterIndex, Vector3 position)
        {
            float distance = Vector3.Distance(EntityCollectionManager.GetEntityByIndex(casterIndex).transform.position, position);
            return !(distance > SO.maxRange);
        }

        public abstract void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions);
        public virtual void AnimationFeedback(){}

        #endregion
    }
}