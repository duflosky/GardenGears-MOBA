using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using Entities.Building;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

public class Tower : Building, IAttackable
{
    [Space]
    [Header("Tower settings")]
    public float detectionRange;
    public List<Entity> enemiesInRange = new List<Entity>();
    public int damage;
    public float delayBeforeAttack;
    public float detectionDelay;
    public float brainSpeed;
    public float timeBetweenShots;
    public LayerMask canBeHitByTowerMask;
    public bool isCycleAttack = false;
    public string enemyUnit;
    private float brainTimer;

    protected override void OnUpdate()
    {
        // Créer des tick pour éviter le saut de frame en plus avec le multi ça risque d'arriver
        brainTimer += Time.deltaTime;
        if (brainTimer > brainSpeed)
        {
            TowerDetection();
            brainTimer = 0;
        }
    }

    private void TowerDetection()
    {
        enemiesInRange.Clear();
        
        var size = Physics.OverlapSphere(transform.position, detectionRange, canBeHitByTowerMask);
        

        foreach (var result in size)
        {
            if (result.CompareTag(enemyUnit))
            {
                enemiesInRange.Add(result.GetComponent<Entity>());
            }
        }

        if (isCycleAttack == false && enemiesInRange.Count > 0)
        {
            StartCoroutine(AttackTarget());
        }
    }

    private IEnumerator AttackTarget()
    {
        isCycleAttack = true;
        
        yield return new WaitForSeconds(detectionDelay);
        
        int[] targetEntity = new[] { enemiesInRange[0].GetComponent<Entity>().entityIndex };
        
        // TODO: Add variable who can store the capacity of the tower
        AttackRPC(3, targetEntity, Array.Empty<Vector3>());
        
        yield return new WaitForSeconds(timeBetweenShots);
        isCycleAttack = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    
    #region Attackable
    
    public bool CanAttack()
    {
        throw new System.NotImplementedException();
    }

    public void RequestSetCanAttack(bool value)
    {
        throw new System.NotImplementedException();
    }

    public void SetCanAttackRPC(bool value)
    {
        throw new System.NotImplementedException();
    }

    public void SyncSetCanAttackRPC(bool value)
    {
        throw new System.NotImplementedException();
    }

    public event GlobalDelegates.BoolDelegate OnSetCanAttack;
    public event GlobalDelegates.BoolDelegate OnSetCanAttackFeedback;
    
    public float GetAttackDamage()
    {
        throw new System.NotImplementedException();
    }

    public void RequestSetAttackDamage(float value)
    {
        throw new System.NotImplementedException();
    }

    public void SyncSetAttackDamageRPC(float value)
    {
        throw new System.NotImplementedException();
    }

    public void SetAttackDamageRPC(float value)
    {
        throw new System.NotImplementedException();
    }

    public event GlobalDelegates.FloatDelegate OnSetAttackDamage;
    public event GlobalDelegates.FloatDelegate OnSetAttackDamageFeedback;
    
    public void RequestAttack(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        throw new System.NotImplementedException();
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

        if (!attackCapacity.TryCast(entityIndex, targetedEntities, targetedPositions)) return;
            
        OnAttack?.Invoke(capacityIndex,targetedEntities,targetedPositions);
        photonView.RPC("SyncAttackRPC",RpcTarget.All,capacityIndex,targetedEntities,targetedPositions);
    }

    public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnAttack;
    public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnAttackFeedback;
    
    public void RequestIncreaseAttackDamage(float value)
    {
        throw new NotImplementedException();
    }

    public void SyncIncreaseAttackDamageRPC(float value)
    {
        throw new NotImplementedException();
    }

    public void IncreaseAttackDamageRPC(float value)
    {
        throw new NotImplementedException();
    }

    public event GlobalDelegates.FloatDelegate OnIncreaseAttackDamage;
    public event GlobalDelegates.FloatDelegate OnIncreaseAttackDamageFeedback;
    
    public void RequestDecreaseAttackDamage(float value)
    {
        throw new NotImplementedException();
    }

    public void SyncDecreaseAttackDamageRPC(float value)
    {
        throw new NotImplementedException();
    }

    public void DecreaseAttackDamageRPC(float value)
    {
        throw new NotImplementedException();
    }

    public event GlobalDelegates.FloatDelegate OnDecreaseAttackDamage;
    public event GlobalDelegates.FloatDelegate OnDecreaseAttackDamageFeedback;
    public void RequestIncreaseAttackSpeed(float value)
    {
        throw new NotImplementedException();
    }

    public void SyncIncreaseAttackSpeedRPC(float value)
    {
        throw new NotImplementedException();
    }

    public void IncreaseAttackSpeedRPC(float value)
    {
        throw new NotImplementedException();
    }

    public event GlobalDelegates.FloatDelegate OnIncreaseAttackSpeed;
    public event GlobalDelegates.FloatDelegate OnIncreaseAttackSpeedFeedback;
    public void RequestDecreaseAttackSpeed(float value)
    {
        throw new NotImplementedException();
    }

    public void SyncDecreaseAttackSpeedRPC(float value)
    {
        throw new NotImplementedException();
    }

    public void DecreaseAttackSpeedRPC(float value)
    {
        throw new NotImplementedException();
    }

    public event GlobalDelegates.FloatDelegate OnDecreaseAttackSpeed;
    public event GlobalDelegates.FloatDelegate OnDecreaseAttackSpeedFeedback;

    #endregion
}