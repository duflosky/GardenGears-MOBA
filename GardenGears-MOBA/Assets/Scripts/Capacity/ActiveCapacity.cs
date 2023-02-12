using GameStates;
using UnityEngine;

namespace Entities.Capacities
{
    public abstract class ActiveCapacity
    {
        public ActiveCapacitySO ActiveCapacitySO;
        public bool OnCooldown;
        public byte IndexOfSOInCollection;
        public Entity Caster;
        public Transform CasterTransform;

        protected int[] TargetsEntityIndexes;
        protected Vector3[] TargetPositions;

        private double _cooldownTimer;

        protected ActiveCapacitySO AssociatedActiveCapacitySO()
        {
            return CapacitySOCollectionManager.GetActiveCapacitySOByIndex(IndexOfSOInCollection);
        }

        public abstract void OnStart();

        #region Cast

        protected virtual void InitiateCooldown()
        {
            _cooldownTimer = ActiveCapacitySO.cooldown;
            OnCooldown = true;

            GameStateMachine.Instance.OnTick += CooldownTimer;
        }

        /// <summary>
        /// Method which update the timer.
        /// </summary>
        protected virtual void CooldownTimer()
        {
            _cooldownTimer -= 1.0 / GameStateMachine.Instance.tickRate;
            if (!(_cooldownTimer <= 0)) return;
            OnCooldown = false;
            GameStateMachine.Instance.OnTick -= CooldownTimer;
        }

        public virtual bool TryCast(int[] targetsEntityIndexes, Vector3[] targetsPositions)
        {
            if (OnCooldown) return false;
            InitiateCooldown();
            TargetsEntityIndexes = targetsEntityIndexes;
            TargetPositions = targetsPositions;
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
            float distance = Vector3.Distance(EntityCollectionManager.GetEntityByIndex(casterIndex).transform.position,
                position);
            return !(distance > ActiveCapacitySO.maxRange);
        }

        public abstract void PlayFeedback(int casterIndex, int[] targetsEntityIndexes, Vector3[] targetPositions);

        public virtual void AnimationFeedback() { }

        #endregion
    }
}