using Entities.Capacities;

public interface IActiveLifeable
{
    bool AttackAffected();
    bool AbilitiesAffected();

    /// <returns>The maxHp of the entity</returns>
    public float GetMaxHp();

    /// <returns>The currentHp of the entity</returns>
    public float GetCurrentHp();

    /// /// <summary>
    /// Sends an RPC to the master to set the entity's maxHp.
    /// </summary>
    /// <param name="value">the value to set it to</param>
    public void RequestSetMaxHp(float value);

    /// <summary>
    /// Sends an RPC to all clients to set the entity's maxHp.
    /// </summary>
    /// <param name="value">the value to set it to</param>
    public void SyncSetMaxHpRPC(float value);

    /// <summary>
    /// Sets the entity's maxHp.
    /// </summary>
    /// <param name="value">the value to set it to</param>
    public void SetMaxHpRPC(float value);

    public event GlobalDelegates.FloatDelegate OnSetMaxHp;
    public event GlobalDelegates.FloatDelegate OnSetMaxHpFeedback;

    /// <summary>
    /// Sends an RPC to the master to increase the entity's maxHp.
    /// </summary>
    /// <param name="amount">the increase amount</param>
    public void RequestIncreaseMaxHp(float amount);

    /// <summary>
    /// Sends an RPC to all clients to increase the entity's maxHp.
    /// </summary>
    /// <param name="amount">the increase amount</param>
    public void SyncIncreaseMaxHpRPC(float amount);

    /// <summary>
    /// Increases the entity's maxHp.
    /// </summary>
    /// <param name="amount">the increase amount</param>
    public void IncreaseMaxHpRPC(float amount);

    public event GlobalDelegates.FloatDelegate OnIncreaseMaxHp;
    public event GlobalDelegates.FloatDelegate OnIncreaseMaxHpFeedback;

    /// <summary>
    /// Sends an RPC to the master to decrease the entity's maxHp.
    /// </summary>
    /// <param name="amount">the decrease amount</param>
    public void RequestDecreaseMaxHp(float amount);

    /// <summary>
    /// Sends an RPC to all clients to decrease the entity's maxHp.
    /// </summary>
    /// <param name="amount">the increase amount</param>
    public void SyncDecreaseMaxHpRPC(float amount);

    /// <summary>
    /// Decreases the entity's maxHp.
    /// </summary>
    /// <param name="amount">the increase amount</param>
    public void DecreaseMaxHpRPC(float amount);

    public event GlobalDelegates.FloatDelegate OnDecreaseMaxHp;
    public event GlobalDelegates.FloatDelegate OnDecreaseMaxHpFeedback;

    /// <summary>
    /// Sends an RPC to the master to set the entity's currentHp.
    /// </summary>
    /// <param name="value">the value to set it to</param>
    public void RequestSetCurrentHp(float value);

    /// <summary>
    /// Sends an RPC to all clients to set the entity's currentHp.
    /// </summary>
    /// <param name="value">the value to set it to</param>
    public void SyncSetCurrentHpRPC(float value);

    /// <summary>
    /// Set the entity's currentHp.
    /// </summary>
    /// <param name="value">the value to set it to</param>
    public void SetCurrentHpRPC(float value);

    public event GlobalDelegates.FloatDelegate OnSetCurrentHp;
    public event GlobalDelegates.FloatDelegate OnSetCurrentHpFeedback;

    /// <summary>
    /// Sends an RPC to the master to increase the entity's currentHp.
    /// </summary>
    /// <param name="amount">the increase amount</param>
    public void RequestIncreaseCurrentHp(float amount);

    /// <summary>
    /// Sends an RPC to all clients to increase the entity's currentHp.
    /// </summary>
    /// <param name="amount">the increase amount</param>
    public void SyncIncreaseCurrentHpRPC(float amount);

    /// <summary>
    /// Increases the entity's currentHp.
    /// </summary>
    /// <param name="amount">the decrease amount</param>
    public void IncreaseCurrentHpRPC(float amount);

    public event GlobalDelegates.FloatDelegate OnIncreaseCurrentHp;
    public event GlobalDelegates.FloatDelegate OnIncreaseCurrentHpFeedback;

    /// <summary>
    /// Sends an RPC to the master to decrease the entity's currentHp.
    /// </summary>
    /// <param name="amount">the decrease amount</param>
    /// <param name="capacity">the capacity receive</param>
    public void RequestDecreaseCurrentHp(float amount);

    /// <summary>
    /// Sends an RPC to all clients to decrease the entity's currentHp.
    /// </summary>
    /// <param name="amount">the decrease amount</param>
    public void SyncDecreaseCurrentHpRPC(float amount);

    /// <summary>
    /// Decreases the entity's currentHp.
    /// </summary>
    /// <param name="amount">the decrease amount</param>
    public void DecreaseCurrentHpRPC(float amount);
    
    public event GlobalDelegates.FloatDelegate OnDecreaseCurrentHp;
    public event GlobalDelegates.FloatDelegate OnDecreaseCurrentHpFeedback;
    
    /// <summary>
    /// Sends an RPC to the master to decrease the entity's currentHp.
    /// </summary>
    /// <param name="amount">the decrease amount</param>
    /// <param name="capacity">the capacity receive</param>
    public void RequestDecreaseCurrentHpByCapacity(float amount, byte capacityIndex);

    /// <summary>
    /// Sends an RPC to all clients to decrease the entity's currentHp.
    /// </summary>
    /// <param name="amount">the decrease amount</param>
    /// <param name="capacity">the capacity receive</param>
    public void SyncDecreaseCurrentHpByCapacityRPC(float amount, byte capacityIndex);

    /// <summary>
    /// Decreases the entity's currentHp.
    /// </summary>
    /// <param name="amount">the decrease amount</param>
    /// <param name="capacity">the capacity receive</param>
    public void DecreaseCurrentHpByCapacityRPC(float amount, byte capacityIndex);
    
    public event GlobalDelegates.FloatCapacityDelegate OnDecreaseCurrentHpCapacity;
    public event GlobalDelegates.FloatCapacityDelegate OnDecreaseCurrentHpCapacityFeedback;
}