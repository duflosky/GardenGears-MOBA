using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Champion : Entity, IMovable
{
    private Rigidbody rb;
    
    
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
        //var so = GameStateMachine.Instance.allChampionsSo[championSoIndex];
        //championSo = so;
        //maxHp = championSo.maxHp;
        //currentHp = maxHp;
        //maxResource = championSo.maxRessource;
        //currentResource = championSo.maxRessource;
        //viewRange = championSo.viewRange;
        //referenceMoveSpeed = championSo.referenceMoveSpeed;
        currentMoveSpeed = referenceMoveSpeed;
        //attackDamage = championSo.attackDamage;
        //attackAbilityIndex = championSo.attackAbilityIndex;
        //abilitiesIndexes = championSo.activeCapacitiesIndexes;
        //ultimateAbilityIndex = championSo.ultimateAbilityIndex;
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
    
    
    public void RequestCast(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
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
}
