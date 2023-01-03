using System;
using System.Collections.Generic;
using Entities;
using Entities.Building;
using Entities.Capacities;
using Entities.Minion;
using GameStates;
using GameStates.States;
using Photon.Pun;
using UnityEngine;

public class Tower : Building, IAttackable
{
    [Space]
    [Header("Tower settings")]
    public float damage;
    public Transform shootSpot;

    [SerializeField] private ActiveCapacitySO attackAbility;
    [SerializeField] private List<Entity> enemiesInRange = new();
    [SerializeField] private LayerMask canBeHitByTowerMask;
    [SerializeField] private Transform baseSpot;
    [SerializeField] private double cooldownCatchMinion;
    
    private double timer;
    private double timerCatchMinion;
    private byte attackAbilityIndex;

    private void OnEnable()
    {
        GameStateMachine.Instance.OnTick += TowerDetection;
    }

    private void OnDisable()
    {
        GameStateMachine.Instance.OnTick -= TowerDetection;
    }
    
    protected override void OnStart()
    {
        base.OnStart();
        attackAbilityIndex = CapacitySOCollectionManager.GetActiveCapacitySOIndex(attackAbility);
    }

    private void TowerDetection()
    {
        timer += 1 / GameStateMachine.Instance.tickRate;
        timerCatchMinion += 1 / GameStateMachine.Instance.tickRate;

        if (timer <= attackAbility.cooldown || timerCatchMinion <= cooldownCatchMinion) return;
        enemiesInRange.Clear();
        foreach (var collider in Physics.OverlapSphere(baseSpot.position, attackAbility.maxRange, canBeHitByTowerMask))
        {
            if (!collider.GetComponent<Entity>()) continue;
            var entity = collider.GetComponent<Entity>();
            if (entity.GetComponent<Tower>()) return;
            if (entity.team == team) continue;
            if (entity is Minion) enemiesInRange.Add(entity);
            else if (entity is Champion) enemiesInRange.Add(entity);
        }

        if (enemiesInRange.Count < 1) return;
        if (cooldownCatchMinion <= timerCatchMinion)
        {
            Entity target = enemiesInRange[0];
            // TODO: Add void effect on the target
            enemiesInRange.Remove(target);
            target.GetComponent<IDeadable>().RequestDie();
            ((InGameState)GameStateMachine.Instance.currentState).AddKill(team);
            timerCatchMinion = 0;
        }
        
        if (enemiesInRange.Count < 1) return;
        int[] targetEntity = { enemiesInRange[0].GetComponent<Entity>().entityIndex };
        timer = 0;
        if (!PhotonNetwork.IsMasterClient) return;
        RequestAttack(attackAbilityIndex, targetEntity, Array.Empty<Vector3>());
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(baseSpot.position, attackAbility.maxRange);
    }
    
    #region Attackable

    private bool canAttack = true;
    
    public bool CanAttack()
    {
        return canAttack;
    }

    public void RequestSetCanAttack(bool value) { }

    public void SetCanAttackRPC(bool value) { }

    public void SyncSetCanAttackRPC(bool value) { }

    public event GlobalDelegates.BoolDelegate OnSetCanAttack;
    public event GlobalDelegates.BoolDelegate OnSetCanAttackFeedback;

    public float GetAttackDamage()
    {
        return damage;
    }

    public void RequestSetAttackDamage(float value) { }

    public void SyncSetAttackDamageRPC(float value) { }

    public void SetAttackDamageRPC(float value) { }

    public event GlobalDelegates.FloatDelegate OnSetAttackDamage;
    public event GlobalDelegates.FloatDelegate OnSetAttackDamageFeedback;
    
    public void RequestAttack(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        photonView.RPC("AttackRPC", RpcTarget.MasterClient, capacityIndex, targetedEntities, targetedPositions);
    }

    [PunRPC]
    public void SyncAttackRPC(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        var attackCapacity = CapacitySOCollectionManager.CreateActiveCapacity(capacityIndex,this);
        attackCapacity.PlayFeedback(capacityIndex,targetedEntities,targetedPositions);
        OnAttackFeedback?.Invoke(capacityIndex,targetedEntities,targetedPositions);
    }

    [PunRPC]
    public void AttackRPC(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        var attackCapacity = CapacitySOCollectionManager.CreateActiveCapacity(capacityIndex,this);

        if (!attackCapacity.TryCast(targetedEntities, targetedPositions)) return;
            
        OnAttack?.Invoke(capacityIndex,targetedEntities,targetedPositions);
        photonView.RPC("SyncAttackRPC",RpcTarget.All,capacityIndex,targetedEntities,targetedPositions);
    }

    public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnAttack;
    public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnAttackFeedback;
    
    public void RequestIncreaseAttackDamage(float value) { }

    public void SyncIncreaseAttackDamageRPC(float value) { }

    public void IncreaseAttackDamageRPC(float value) { }

    public event GlobalDelegates.FloatDelegate OnIncreaseAttackDamage;
    public event GlobalDelegates.FloatDelegate OnIncreaseAttackDamageFeedback;
    
    public void RequestDecreaseAttackDamage(float value) { }

    public void SyncDecreaseAttackDamageRPC(float value) { }

    public void DecreaseAttackDamageRPC(float value) { }

    public event GlobalDelegates.FloatDelegate OnDecreaseAttackDamage;
    public event GlobalDelegates.FloatDelegate OnDecreaseAttackDamageFeedback;
    
    public void RequestIncreaseAttackSpeed(float value) { }

    public void SyncIncreaseAttackSpeedRPC(float value) { }

    public void IncreaseAttackSpeedRPC(float value) { }

    public event GlobalDelegates.FloatDelegate OnIncreaseAttackSpeed;
    public event GlobalDelegates.FloatDelegate OnIncreaseAttackSpeedFeedback;
    
    public void RequestDecreaseAttackSpeed(float value) { }

    public void SyncDecreaseAttackSpeedRPC(float value) { }

    public void DecreaseAttackSpeedRPC(float value) { }

    public event GlobalDelegates.FloatDelegate OnDecreaseAttackSpeed;
    public event GlobalDelegates.FloatDelegate OnDecreaseAttackSpeedFeedback;

    #endregion
}