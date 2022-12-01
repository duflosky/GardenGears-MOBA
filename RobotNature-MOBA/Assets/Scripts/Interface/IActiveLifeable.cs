
public interface IActiveLifeable
{
    bool AttackAffected();
    bool AbilitiesAffected();
    
    /// <summary>
    /// Sends an RPC to the master to decrease the entity's currentHp.
    /// </summary>
    /// <param name="amount">the decrease amount</param>
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
}