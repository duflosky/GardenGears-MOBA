using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using Entities.Champion;
using GameStates;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Champion : Entity, IMovable, ICastable, IActiveLifeable
{
    private Rigidbody rb;
    private ChampionSO championSo;
    
    
    protected override void OnStart()
    {
        base.OnStart();
        rb = GetComponent<Rigidbody>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        Move();
    }

    public void ApplyChampionSO(byte championSoIndex)
    {
        var so = GameStateMachine.Instance.allChampionsSo[championSoIndex];
        championSo = so;
        maxHp = championSo.maxHp;
        currentHp = maxHp;
        //maxResource = championSo.maxRessource;
        //currentResource = championSo.maxRessource;
        //viewRange = championSo.viewRange;
        referenceMoveSpeed = championSo.referenceMoveSpeed;
        currentMoveSpeed = referenceMoveSpeed;
        //attackDamage = championSo.attackDamage;
        //attackAbilityIndex = championSo.attackAbilityIndex;
        abilitiesIndexes = championSo.activeCapacitiesIndexes;
        ultimateAbilityIndex = championSo.ultimateAbilityIndex;
    }

    #region Mouvement
    [Header("=== MOUVEMENT")]
    private Vector3 lastDir;
    private bool isMoving;

    [SerializeField] float referenceMoveSpeed;
    float currentMoveSpeed = 3;

    public void SetMoveDirection(Vector3 dir)
    {
        lastDir = dir;
        isMoving = (dir != Vector3.zero) ;
    }

    void Move()
    {
        rb.velocity = lastDir * currentMoveSpeed;
    }
    #endregion

    #region Cast
    [Header("=== CAST")]
    public byte[] abilitiesIndexes = new byte[2];
    public byte ultimateAbilityIndex;
        
    public bool canCast;


    public bool CanCast()
    {
        return canCast;
    }

    public void RequestSetCanCast(bool value)
    {
        throw new System.NotImplementedException();
    }

    [PunRPC]
    public void SetCanCastRPC(bool value)
    {
        canCast = value;
        OnSetCanCast?.Invoke(value);
        photonView.RPC("SyncCastRPC",RpcTarget.All,canCast);
    }

    [PunRPC]
    public void SyncSetCanCastRPC(bool value)
    {
        canCast = value;
        OnSetCanCastFeedback?.Invoke(value);
    }

    public event GlobalDelegates.BoolDelegate OnSetCanCast;
    public event GlobalDelegates.BoolDelegate OnSetCanCastFeedback;

    public void RequestCast(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        Debug.Log(targetedPositions);
        photonView.RPC("CastRPC",RpcTarget.MasterClient,capacityIndex,targetedEntities,targetedPositions);
    }
        
    [PunRPC]
    public void CastRPC(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        var activeCapacity = CapacitySOCollectionManager.CreateActiveCapacity(capacityIndex,this);
            
        if (!activeCapacity.TryCast(entityIndex, targetedEntities, targetedPositions)) return;
            
        OnCast?.Invoke(capacityIndex,targetedEntities,targetedPositions);
        photonView.RPC("SyncCastRPC",RpcTarget.All,capacityIndex,targetedEntities,targetedPositions);

    }

    [PunRPC]
    public void SyncCastRPC(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        var activeCapacity = CapacitySOCollectionManager.CreateActiveCapacity(capacityIndex,this);
        activeCapacity.PlayFeedback(capacityIndex,targetedEntities,targetedPositions);
        OnCastFeedback?.Invoke(capacityIndex,targetedEntities,targetedPositions,activeCapacity);
    }
    
    public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnCast;
    public event GlobalDelegates.ByteIntArrayVector3ArrayCapacityDelegate OnCastFeedback;
    #endregion

    #region ActiveLife

    [SerializeField] private bool attackAffect;
    [SerializeField] private bool abilitiesAffect;
    private float maxHp;
    private float currentHp;
    
    public bool AttackAffected()
    {
        return attackAffect;
    }

    public bool AbilitiesAffected()
    {
        return abilitiesAffect;
    }

    public void RequestDecreaseCurrentHp(float amount)
    {
        throw new NotImplementedException();
    }
    
    [PunRPC]
    public void DecreaseCurrentHpRPC(float amount)
    {
        currentHp -= amount;
        OnDecreaseCurrentHp?.Invoke(amount);
        photonView.RPC("SyncDecreaseCurrentHpRPC",RpcTarget.All,currentHp);
        if (currentHp <= 0)
        {
            currentHp = 0;
            Debug.Log("Die");
            gameObject.SetActive(false);
            //TODO : RequestDie();
        }
    }

    [PunRPC]
    public void SyncDecreaseCurrentHpRPC(float amount)
    {
        throw new NotImplementedException();
    }


    public event GlobalDelegates.FloatDelegate OnDecreaseCurrentHp;
    public event GlobalDelegates.FloatDelegate OnDecreaseCurrentHpFeedback;

    #endregion
}
