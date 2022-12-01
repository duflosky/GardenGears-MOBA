using UnityEngine;

namespace Entities
{
    public interface IAttackable
    {
        
        /// <returns>true if the entity can attack, false if not</returns>
        public bool CanAttack();
        /// <summary>
        /// Sends an RPC to the master to set if the entity can attack.
        /// </summary>
        public void RequestSetCanAttack(bool value);
        /// <summary>
        /// Sets if the entity can attack.
        /// </summary>
        public void SetCanAttackRPC(bool value);
        /// <summary>
        /// Sends an RPC to all clients to set if the entity can attack.
        /// </summary>
        public void SyncSetCanAttackRPC(bool value);

        public event GlobalDelegates.BoolDelegate OnSetCanAttack;
        public event GlobalDelegates.BoolDelegate OnSetCanAttackFeedback;
        
        /// <returns>the entity's attack damage</returns>
        public float GetAttackDamage();
        /// <summary>
        /// Sends an RPC to the master to set the entity's attack damage.
        /// </summary>
        public void RequestSetAttackDamage(float value);
        /// <summary>
        /// Sends an RPC to all clients to set the entity's attack damage.
        /// </summary>
        public void SyncSetAttackDamageRPC(float value);
        /// <summary>
        /// Sets the entity's attack damage.
        /// </summary>
        public void SetAttackDamageRPC(float value);

        public event GlobalDelegates.FloatDelegate OnSetAttackDamage;
        public event GlobalDelegates.FloatDelegate OnSetAttackDamageFeedback;
        
        /// <summary>
        /// Sends an RPC to the master to Attack.
        /// </summary>
        /// <param name="capacityIndex">the index on the CapacitySOCollectionManager of the activeCapacitySO to Attack</param>
        /// <param name="targetedEntities">the entities targeted by the activeCapacity</param>
        /// <param name="targetedPositions">the positions targeted by  the activeCapacities</param>
        public void RequestAttack(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions);
        /// <summary>
        /// Sends an RPC to all clients to Attack an ActiveCapacity.
        /// </summary>
        /// <param name="capacityIndex">the index on the CapacitySOCollectionManager of the activeCapacitySO to Attack</param>
        /// <param name="targetedEntities">the entities targeted by the activeCapacity</param>
        /// <param name="targetedPositions">the positions targeted by  the activeCapacities</param>
        public void SyncAttackRPC(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions);
        /// <summary>
        /// Attacks an ActiveCapacity.
        /// </summary>
        /// <param name="capacityIndex">the index on the CapacitySOCollectionManager of the activeCapacitySO to Attack</param>
        /// <param name="targetedEntities">the entities targeted by the activeCapacity</param>
        /// <param name="targetedPositions">the positions targeted by  the activeCapacities</param>
        public void AttackRPC(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions);

        public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnAttack;
        public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnAttackFeedback;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void RequestIncreaseAttackDamage(float value);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SyncIncreaseAttackDamageRPC(float value);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void IncreaseAttackDamageRPC(float value);
        
        public event GlobalDelegates.FloatDelegate OnIncreaseAttackDamage;
        public event GlobalDelegates.FloatDelegate OnIncreaseAttackDamageFeedback;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void RequestDecreaseAttackDamage(float value);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SyncDecreaseAttackDamageRPC(float value);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void DecreaseAttackDamageRPC(float value);
        
        public event GlobalDelegates.FloatDelegate OnDecreaseAttackDamage;
        public event GlobalDelegates.FloatDelegate OnDecreaseAttackDamageFeedback;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void RequestIncreaseAttackSpeed(float value);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SyncIncreaseAttackSpeedRPC(float value);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void IncreaseAttackSpeedRPC(float value);
        
        public event GlobalDelegates.FloatDelegate OnIncreaseAttackSpeed;
        public event GlobalDelegates.FloatDelegate OnIncreaseAttackSpeedFeedback;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void RequestDecreaseAttackSpeed(float value);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SyncDecreaseAttackSpeedRPC(float value);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void DecreaseAttackSpeedRPC(float value);
        
        public event GlobalDelegates.FloatDelegate OnDecreaseAttackSpeed;
        public event GlobalDelegates.FloatDelegate OnDecreaseAttackSpeedFeedback;
    }
}
