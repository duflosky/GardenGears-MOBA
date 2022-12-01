using System.Collections.Generic;
using System.Linq;
using Entities;
using Entities.Capacities;
using Entities.Inventory;
using GameStates;
using Entities.Champion;
using Items;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Champion : Entity, IMovable, IInventoryable, IResourceable, ICastable, IActiveLifeable, IDeadable
{
    private Rigidbody rb;
    private ChampionSO championSo;

    [SerializeReference] public List<Item> items = new List<Item>();

    public float maxResource;
    public float currentResource;

    private UI.InGame.UIManager uiManager;

    protected override void OnStart()
    {
        base.OnStart();
        rb = GetComponent<Rigidbody>();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        Move();
        Rotate();
    }

    public void ApplyChampionSO(byte championSoIndex, Enums.Team newTeam)
    {
        var so = GameStateMachine.Instance.allChampionsSo[championSoIndex];
        championSo = so;
        maxHp = championSo.maxHp;
        currentHp = maxHp;
        uiManager = UI.InGame.UIManager.Instance;
        maxResource = so.maxRessource;
        currentResource = so.maxRessource;
        //viewRange = championSo.viewRange;
        referenceMoveSpeed = championSo.referenceMoveSpeed;
        currentMoveSpeed = referenceMoveSpeed;
        //attackDamage = championSo.attackDamage;
        //attackAbilityIndex = championSo.attackAbilityIndex;
        // TODO : Instantiate mesh champion ? 
        Debug.Log(
            $"ChampionSO: {championSo}, activeCapacitiesIndexes.Lenght: {championSo.activeCapacitiesIndexes.Length}");
        abilitiesIndexes = championSo.activeCapacitiesIndexes;
        ultimateAbilityIndex = championSo.ultimateAbilityIndex;

        team = newTeam;

        Transform pos = transform;
        switch (team)
        {
            case Enums.Team.Team1:
            {
                for (int i = 0; i < MapLoaderManager.Instance.firstTeamBasePoint.Length; i++)
                {
                    if (MapLoaderManager.Instance.firstTeamBasePoint[i].champion == null)
                    {
                        pos = MapLoaderManager.Instance.firstTeamBasePoint[i].position;
                        MapLoaderManager.Instance.firstTeamBasePoint[i].champion = this;
                        break;
                    }
                }

                break;
            }
            case Enums.Team.Team2:
            {
                for (int i = 0; i < MapLoaderManager.Instance.secondTeamBasePoint.Length; i++)
                {
                    if (MapLoaderManager.Instance.secondTeamBasePoint[i].champion == null)
                    {
                        pos = MapLoaderManager.Instance.secondTeamBasePoint[i].position;
                        MapLoaderManager.Instance.secondTeamBasePoint[i].champion = this;
                        break;
                    }
                }

                break;
            }
            default:
                Debug.LogError("Team is not valid.");
                pos = transform;
                break;
        }

        respawnPos = transform.position = pos.position;

        if (uiManager != null)
        {
            uiManager.InstantiateHealthBarForEntity(entityIndex);
            uiManager.InstantiateResourceBarForEntity(entityIndex);
        }
    }

    #region Moveable

    [Header("=== MOUVEMENT")] private Vector3 lastDir;
    private bool isMoving;

    [SerializeField] float referenceMoveSpeed;
    float currentMoveSpeed = 3;

    public Transform rotateParent;
    public float currentRotateSpeed;
    private Vector3 rotateDirection;

    public void SetMoveDirection(Vector3 dir)
    {
        lastDir = dir;
        isMoving = (dir != Vector3.zero);
    }
    
    void Move()
    {
        rb.velocity = lastDir * currentMoveSpeed;
    }

    private void RotateMath()
    {
        if (!photonView.IsMine) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out var hit, float.PositiveInfinity)) return;

        rotateDirection = -(transform.position - hit.point);
        rotateDirection.y = 0;
    }

    private void Rotate()
    {
        RotateMath();
        rotateParent.transform.rotation = Quaternion.Lerp(rotateParent.transform.rotation,Quaternion.LookRotation(rotateDirection),Time.deltaTime * currentRotateSpeed);
    }

    public float GetReferenceMoveSpeed()
    {
        return referenceMoveSpeed;
    }

    public float GetCurrentMoveSpeed()
    {
        return currentMoveSpeed;
    }

    public void RequestSetReferenceMoveSpeed(float value)
    {
        photonView.RPC("SetReferenceMoveSpeedRPC", RpcTarget.MasterClient, value);
    }

    [PunRPC]
    public void SyncSetReferenceMoveSpeedRPC(float value)
    {
        referenceMoveSpeed = value;
        OnSetReferenceMoveSpeedFeedback?.Invoke(value);
    }

    [PunRPC]
    public void SetReferenceMoveSpeedRPC(float value)
    {
        referenceMoveSpeed = value;
        OnSetReferenceMoveSpeed?.Invoke(value);
        photonView.RPC("SyncSetReferenceMoveSpeedRPC", RpcTarget.All, referenceMoveSpeed);
    }

    public event GlobalDelegates.FloatDelegate OnSetReferenceMoveSpeed;
    public event GlobalDelegates.FloatDelegate OnSetReferenceMoveSpeedFeedback;

    public void RequestIncreaseReferenceMoveSpeed(float amount)
    {
        photonView.RPC("IncreaseReferenceMoveSpeedRPC", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    public void SyncIncreaseReferenceMoveSpeedRPC(float amount)
    {
        referenceMoveSpeed = amount;
        OnIncreaseReferenceMoveSpeedFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void IncreaseReferenceMoveSpeedRPC(float amount)
    {
        referenceMoveSpeed += amount;
        OnIncreaseReferenceMoveSpeed?.Invoke(amount);
        photonView.RPC("SyncIncreaseReferenceMoveSpeedRPC", RpcTarget.All, referenceMoveSpeed);
    }

    public event GlobalDelegates.FloatDelegate OnIncreaseReferenceMoveSpeed;
    public event GlobalDelegates.FloatDelegate OnIncreaseReferenceMoveSpeedFeedback;

    public void RequestDecreaseReferenceMoveSpeed(float amount)
    {
        photonView.RPC("DecreaseReferenceMoveSpeedRPC", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    public void SyncDecreaseReferenceMoveSpeedRPC(float amount)
    {
        referenceMoveSpeed = amount;
        OnDecreaseReferenceMoveSpeedFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void DecreaseReferenceMoveSpeedRPC(float amount)
    {
        referenceMoveSpeed -= amount;
        OnDecreaseReferenceMoveSpeed?.Invoke(amount);
        photonView.RPC("SyncDecreaseReferenceMoveSpeedRPC", RpcTarget.All, referenceMoveSpeed);
    }

    public event GlobalDelegates.FloatDelegate OnDecreaseReferenceMoveSpeed;
    public event GlobalDelegates.FloatDelegate OnDecreaseReferenceMoveSpeedFeedback;

    public void RequestSetCurrentMoveSpeed(float value)
    {
        photonView.RPC("SetCurrentMoveSpeedRPC", RpcTarget.MasterClient, value);
    }

    [PunRPC]
    public void SyncSetCurrentMoveSpeedRPC(float value)
    {
        currentMoveSpeed = value;
        OnSetCurrentMoveSpeedFeedback?.Invoke(value);
    }

    [PunRPC]
    public void SetCurrentMoveSpeedRPC(float value)
    {
        currentMoveSpeed = value;
        OnSetCurrentMoveSpeed?.Invoke(value);
        photonView.RPC("SyncSetCurrentMoveSpeedRPC", RpcTarget.All, currentMoveSpeed);
    }

    public event GlobalDelegates.FloatDelegate OnSetCurrentMoveSpeed;
    public event GlobalDelegates.FloatDelegate OnSetCurrentMoveSpeedFeedback;

    public void RequestIncreaseCurrentMoveSpeed(float amount)
    {
        photonView.RPC("IncreaseCurrentMoveSpeedRPC", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    public void SyncIncreaseCurrentMoveSpeedRPC(float amount)
    {
        currentMoveSpeed = amount;
        OnIncreaseCurrentMoveSpeedFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void IncreaseCurrentMoveSpeedRPC(float amount)
    {
        currentMoveSpeed += amount;
        OnIncreaseCurrentMoveSpeed?.Invoke(amount);
        photonView.RPC("SyncIncreaseCurrentMoveSpeedRPC", RpcTarget.All, currentMoveSpeed);
    }

    public event GlobalDelegates.FloatDelegate OnIncreaseCurrentMoveSpeed;
    public event GlobalDelegates.FloatDelegate OnIncreaseCurrentMoveSpeedFeedback;

    public void RequestDecreaseCurrentMoveSpeed(float amount)
    {
        photonView.RPC("DecreaseCurrentMoveSpeedRPC", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    public void SyncDecreaseCurrentMoveSpeedRPC(float amount)
    {
        currentMoveSpeed = amount;
        OnDecreaseCurrentMoveSpeedFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void DecreaseCurrentMoveSpeedRPC(float amount)
    {
        currentMoveSpeed -= amount;
        OnDecreaseCurrentMoveSpeed?.Invoke(amount);
        photonView.RPC("SyncDecreaseCurrentMoveSpeedRPC", RpcTarget.All, currentMoveSpeed);
    }

    public event GlobalDelegates.FloatDelegate OnDecreaseCurrentMoveSpeed;
    public event GlobalDelegates.FloatDelegate OnDecreaseCurrentMoveSpeedFeedback;

    #endregion

    #region Cast

    [Header("=== CAST")] public byte[] abilitiesIndexes = new byte[2];
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
        photonView.RPC("SyncCastRPC", RpcTarget.All, canCast);
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
        photonView.RPC("CastRPC", RpcTarget.MasterClient, capacityIndex, targetedEntities, targetedPositions);
    }

    [PunRPC]
    public void CastRPC(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        var activeCapacity = CapacitySOCollectionManager.CreateActiveCapacity(capacityIndex, this);
        if (!activeCapacity.TryCast(entityIndex, targetedEntities, targetedPositions)) return;

        OnCast?.Invoke(capacityIndex, targetedEntities, targetedPositions);
        photonView.RPC("SyncCastRPC", RpcTarget.All, capacityIndex, targetedEntities, targetedPositions);
    }

    [PunRPC]
    public void SyncCastRPC(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
    {
        var activeCapacity = CapacitySOCollectionManager.CreateActiveCapacity(capacityIndex, this);
        activeCapacity.PlayFeedback(capacityIndex, targetedEntities, targetedPositions);
        OnCastFeedback?.Invoke(capacityIndex, targetedEntities, targetedPositions, activeCapacity);
    }

    public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnCast;
    public event GlobalDelegates.ByteIntArrayVector3ArrayCapacityDelegate OnCastFeedback;

    #endregion

    #region ActiveLife

    [SerializeField] private bool attackAffect;
    [SerializeField] private bool abilitiesAffect;
    private float maxHp;
    private float currentHp;

    public float GetMaxHp()
    {
        return maxHp;
    }

    public float GetCurrentHp()
    {
        return currentHp;
    }

    public bool AttackAffected()
    {
        return attackAffect;
    }

    public bool AbilitiesAffected()
    {
        return abilitiesAffect;
    }

    public void RequestSetMaxHp(float value)
    {
        photonView.RPC("SetMaxHpRPC", RpcTarget.MasterClient, value);
    }

    [PunRPC]
    public void SyncSetMaxHpRPC(float value)
    {
        maxHp = value;
        currentHp = value;
        OnSetMaxHpFeedback?.Invoke(value);
    }

    [PunRPC]
    public void SetMaxHpRPC(float value)
    {
        maxHp = value;
        currentHp = value;
        OnSetMaxHp?.Invoke(value);
        photonView.RPC("SyncSetMaxHpRPC", RpcTarget.All, maxHp);
    }

    public event GlobalDelegates.FloatDelegate OnSetMaxHp;
    public event GlobalDelegates.FloatDelegate OnSetMaxHpFeedback;

    public void RequestIncreaseMaxHp(float amount)
    {
        photonView.RPC("IncreaseMaxHpRPC", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    public void SyncIncreaseMaxHpRPC(float amount)
    {
        maxHp = amount;
        currentHp = amount;
        OnIncreaseMaxHpFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void IncreaseMaxHpRPC(float amount)
    {
        maxHp += amount;
        currentHp = amount;
        if (maxHp < currentHp) currentHp = maxHp;
        OnIncreaseMaxHp?.Invoke(amount);
        photonView.RPC("SyncIncreaseMaxHpRPC", RpcTarget.All, maxHp);
    }

    public event GlobalDelegates.FloatDelegate OnIncreaseMaxHp;
    public event GlobalDelegates.FloatDelegate OnIncreaseMaxHpFeedback;

    public void RequestDecreaseMaxHp(float amount)
    {
        photonView.RPC("DecreaseMaxHpRPC", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    public void SyncDecreaseMaxHpRPC(float amount)
    {
        maxHp = amount;
        if (maxHp < currentHp) currentHp = maxHp;
        OnDecreaseMaxHpFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void DecreaseMaxHpRPC(float amount)
    {
        maxHp -= amount;
        if (maxHp < currentHp) currentHp = maxHp;
        OnDecreaseMaxHp?.Invoke(amount);
        photonView.RPC("SyncDecreaseMaxHpRPC", RpcTarget.All, maxHp);
    }

    public event GlobalDelegates.FloatDelegate OnDecreaseMaxHp;
    public event GlobalDelegates.FloatDelegate OnDecreaseMaxHpFeedback;

    public void RequestSetCurrentHp(float value)
    {
        photonView.RPC("SetCurrentHpRPC", RpcTarget.MasterClient, value);
    }

    [PunRPC]
    public void SyncSetCurrentHpRPC(float value)
    {
        currentHp = value;
        OnSetCurrentHpFeedback?.Invoke(value);
    }

    [PunRPC]
    public void SetCurrentHpRPC(float value)
    {
        currentHp = value;
        OnSetCurrentHp?.Invoke(value);
        photonView.RPC("SyncSetCurrentHpRPC", RpcTarget.All, value);
    }

    public event GlobalDelegates.FloatDelegate OnSetCurrentHp;
    public event GlobalDelegates.FloatDelegate OnSetCurrentHpFeedback;

    public void RequestIncreaseCurrentHp(float amount)
    {
        photonView.RPC("IncreaseCurrentHpRPC", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    public void SyncIncreaseCurrentHpRPC(float amount)
    {
        currentHp = amount;
        OnIncreaseCurrentHpFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void IncreaseCurrentHpRPC(float amount)
    {
        currentHp += amount;
        if (currentHp > maxHp) currentHp = maxHp;
        OnIncreaseCurrentHp?.Invoke(amount);
        photonView.RPC("SyncIncreaseCurrentHpRPC", RpcTarget.All, currentHp);
    }

    public event GlobalDelegates.FloatDelegate OnIncreaseCurrentHp;
    public event GlobalDelegates.FloatDelegate OnIncreaseCurrentHpFeedback;

    public void RequestDecreaseCurrentHp(float amount)
    {
        photonView.RPC("DecreaseCurrentHpRPC", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    public void DecreaseCurrentHpRPC(float amount)
    {
        currentHp = amount;
        if (currentHp <= 0)
        {
            currentHp = 0;
            RequestDie();
        }

        OnDecreaseCurrentHpFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void SyncDecreaseCurrentHpRPC(float amount)
    {
        currentHp -= amount;
        OnDecreaseCurrentHp?.Invoke(amount);
        photonView.RPC("SyncDecreaseCurrentHpRPC", RpcTarget.All, currentHp);
    }


    public event GlobalDelegates.FloatDelegate OnDecreaseCurrentHp;
    public event GlobalDelegates.FloatDelegate OnDecreaseCurrentHpFeedback;

    #endregion

    #region Inventoryable

    public Item[] GetItems()
    {
        return items.ToArray();
    }

    public Item GetItem(int index)
    {
        if (index < 0 || index >= 3) return null;
        return items[index];
    }

    public Item GetItemOfSo(int soIndex)
    {
        return items.FirstOrDefault(item => item.indexOfSOInCollection == soIndex);
    }

    public void RequestAddItem(byte index)
    {
        photonView.RPC("AddItemRPC", RpcTarget.MasterClient, index);
    }

    [PunRPC]
    public void AddItemRPC(byte index)
    {
        var itemSo = ItemCollectionManager.Instance.GetItemSObyIndex(index);
        if (itemSo.consumable)
        {
            var contains = false;
            foreach (var item in items.Where(item => item.indexOfSOInCollection == index))
            {
                contains = true;
            }

            if (!contains && items.Count >= 3) return;
            photonView.RPC("SyncAddItemRPC", RpcTarget.All, index);
            return;
        }

        if (items.Count >= 3) return;
        photonView.RPC("SyncAddItemRPC", RpcTarget.All, index);
    }

    [PunRPC]
    public void SyncAddItemRPC(byte index)
    {
        var item = ItemCollectionManager.Instance.CreateItem(index, this);
        if (item == null) return;
        if (!items.Contains(item)) items.Add(item);
        if (PhotonNetwork.IsMasterClient)
        {
            item.OnItemAddedToInventory(this);
            OnAddItem?.Invoke(index);
        }

        item.OnItemAddedToInventoryFeedback(this);
        OnAddItemFeedback?.Invoke(index);
    }

    public event GlobalDelegates.ByteDelegate OnAddItem;
    public event GlobalDelegates.ByteDelegate OnAddItemFeedback;

    /// <param name="index">index of Item in this entity's inventory (not in item Collection)</param>
    public void RequestRemoveItem(byte index)
    {
        photonView.RPC("RemoveItemRPC", RpcTarget.MasterClient, index);
    }

    /// <param name="item">Item to remove from this entity's inventory</param>
    public void RequestRemoveItem(Item item)
    {
        if (!items.Contains(item)) return;
        RequestRemoveItem((byte)items.IndexOf(item));
    }

    [PunRPC]
    public void RemoveItemRPC(byte index)
    {
        photonView.RPC("SyncRemoveItemRPC", RpcTarget.All, index);
    }

    public void RemoveItemRPC(Item item)
    {
        if (!items.Contains(item)) return;
        var index = items.IndexOf(item);
        RemoveItemRPC((byte)index);
    }

    [PunRPC]
    public void SyncRemoveItemRPC(byte index)
    {
        if (index >= items.Count) return;
        var item = items[index];
        items.Remove(item);
        if (PhotonNetwork.IsMasterClient)
        {
            item.OnItemRemovedFromInventory(this);
            OnRemoveItem?.Invoke(index);
        }

        item.OnItemRemovedFromInventoryFeedback(this);
        OnRemoveItemFeedback?.Invoke(index);
    }

    public event GlobalDelegates.ByteDelegate OnRemoveItem;
    public event GlobalDelegates.ByteDelegate OnRemoveItemFeedback;

    public void RequestActivateItem(byte itemIndexInInventory, int[] selectedEntities, Vector3[] positions)
    {
        if (itemIndexInInventory >= items.Count) return;
        photonView.RPC("ActivateItemRPC", RpcTarget.MasterClient, itemIndexInInventory, selectedEntities, positions);
    }

    [PunRPC]
    public void ActivateItemRPC(byte itemIndexInInventory, int[] selectedEntities, Vector3[] positions)
    {
        if (itemIndexInInventory >= items.Count) return;
        var item = items[itemIndexInInventory];
        if (item == null) return;

        var successesActives = new bool[item.AssociatedItemSO().activeCapacitiesIndexes.Length];
        var bytes = item.AssociatedItemSO().activeCapacitiesIndexes;
        for (var i = 0; i < bytes.Length; i++)
        {
            var capacityIndex = bytes[i];
            var activeCapacity = CapacitySOCollectionManager.CreateActiveCapacity(capacityIndex, this);
            successesActives[i] = activeCapacity.TryCast(entityIndex, selectedEntities, positions);
        }

        items[itemIndexInInventory].OnItemActivated(selectedEntities, positions);
        OnActivateItem?.Invoke(itemIndexInInventory, selectedEntities, positions);
        photonView.RPC("SyncActivateItemRPC", RpcTarget.All, itemIndexInInventory, selectedEntities, positions,
            successesActives.ToArray());
    }

    [PunRPC]
    public void SyncActivateItemRPC(byte itemIndexInInventory, int[] selectedEntities, Vector3[] positions,
        bool[] castSuccess)
    {
        if (itemIndexInInventory >= items.Count) return;
        var item = items[itemIndexInInventory];
        if (items[itemIndexInInventory] == null) return;
        var bytes = item.AssociatedItemSO().activeCapacitiesIndexes;
        for (var index = 0; index < bytes.Length; index++)
        {
            var capacityIndex = bytes[index];
            var activeCapacity = CapacitySOCollectionManager.CreateActiveCapacity(capacityIndex, this);
            if (castSuccess[index]) activeCapacity.PlayFeedback(entityIndex, selectedEntities, positions);
        }

        items[itemIndexInInventory].OnItemActivatedFeedback(selectedEntities, positions);
        OnActivateItemFeedback?.Invoke(itemIndexInInventory, selectedEntities, positions, castSuccess);
    }

    public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnActivateItem;
    public event GlobalDelegates.ByteIntArrayVector3ArrayBoolArrayDelegate OnActivateItemFeedback;

    #endregion

    #region Resourceable

    public float GetMaxResource()
    {
        return maxResource;
    }

    public float GetCurrentResource()
    {
        return currentResource;
    }

    public float GetCurrentResourcePercent()
    {
        return currentResource / maxResource * 100;
    }

    public void RequestSetMaxResource(float value)
    {
        photonView.RPC("SetMaxResourceRPC", RpcTarget.MasterClient, value);
    }

    [PunRPC]
    public void SyncSetMaxResourceRPC(float value)
    {
        maxResource = value;
        OnSetMaxResourceFeedback?.Invoke(value);
    }

    [PunRPC]
    public void SetMaxResourceRPC(float value)
    {
        maxResource = value;
        OnSetMaxResource?.Invoke(value);
        photonView.RPC("SyncSetMaxResourceRPC", RpcTarget.All, value);
    }

    public event GlobalDelegates.FloatDelegate OnSetMaxResource;
    public event GlobalDelegates.FloatDelegate OnSetMaxResourceFeedback;

    public void RequestIncreaseMaxResource(float amount)
    {
        photonView.RPC("IncreaseMaxResourceRPC", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    public void SyncIncreaseMaxResourceRPC(float amount)
    {
        maxResource = amount;
        OnIncreaseMaxResourceFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void IncreaseMaxResourceRPC(float amount)
    {
        maxResource += amount;
        OnIncreaseMaxResource?.Invoke(amount);
        photonView.RPC("SyncIncreaseMaxResourceRPC", RpcTarget.All, amount);
    }

    public event GlobalDelegates.FloatDelegate OnIncreaseMaxResource;
    public event GlobalDelegates.FloatDelegate OnIncreaseMaxResourceFeedback;

    public void RequestDecreaseMaxResource(float amount)
    {
        photonView.RPC("DecreaseMaxResourceRPC", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    public void SyncDecreaseMaxResourceRPC(float amount)
    {
        maxResource = amount;
        OnDecreaseMaxResourceFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void DecreaseMaxResourceRPC(float amount)
    {
        maxResource -= amount;
        OnDecreaseMaxResource?.Invoke(amount);
        photonView.RPC("SyncDecreaseMaxResourceRPC", RpcTarget.All, amount);
    }

    public event GlobalDelegates.FloatDelegate OnDecreaseMaxResource;
    public event GlobalDelegates.FloatDelegate OnDecreaseMaxResourceFeedback;

    public void RequestSetCurrentResource(float value)
    {
        photonView.RPC("SetCurrentResourceRPC", RpcTarget.MasterClient, value);
    }

    [PunRPC]
    public void SyncSetCurrentResourceRPC(float value)
    {
        currentResource = value;
        OnSetCurrentResourceFeedback?.Invoke(value);
    }

    [PunRPC]
    public void SetCurrentResourceRPC(float value)
    {
        currentResource = value;
        OnSetCurrentResource?.Invoke(value);
        photonView.RPC("SyncSetCurrentResourceRPC", RpcTarget.All, value);
    }

    public event GlobalDelegates.FloatDelegate OnSetCurrentResource;
    public event GlobalDelegates.FloatDelegate OnSetCurrentResourceFeedback;

    public void RequestIncreaseCurrentResource(float amount)
    {
        photonView.RPC("IncreaseCurrentResourceRPC", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    public void SyncIncreaseCurrentResourceRPC(float amount)
    {
        currentResource = amount;
        OnIncreaseCurrentResourceFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void IncreaseCurrentResourceRPC(float amount)
    {
        currentResource += amount;
        OnIncreaseCurrentResource?.Invoke(amount);
        photonView.RPC("SyncIncreaseCurrentResourceRPC", RpcTarget.All, amount);
    }

    public event GlobalDelegates.FloatDelegate OnIncreaseCurrentResource;
    public event GlobalDelegates.FloatDelegate OnIncreaseCurrentResourceFeedback;

    public void RequestDecreaseCurrentResource(float amount)
    {
        photonView.RPC("DecreaseCurrentResourceRPC", RpcTarget.MasterClient, amount);
    }

    [PunRPC]
    public void SyncDecreaseCurrentResourceRPC(float amount)
    {
        currentResource = amount;
        OnDecreaseCurrentResourceFeedback?.Invoke(amount);
    }

    [PunRPC]
    public void DecreaseCurrentResourceRPC(float amount)
    {
        currentResource -= amount;
        OnDecreaseCurrentResource?.Invoke(amount);
        photonView.RPC("SyncDecreaseCurrentResourceRPC", RpcTarget.All, amount);
    }

    public event GlobalDelegates.FloatDelegate OnDecreaseCurrentResource;
    public event GlobalDelegates.FloatDelegate OnDecreaseCurrentResourceFeedback;

    #endregion

    #region Deadable

    public bool isAlive;
    public bool canDie;

    public float respawnDuration = 3;
    private double respawnTimer;

    private Vector3 respawnPos;

    public bool IsAlive()
    {
        return isAlive;
    }

    public bool CanDie()
    {
        return canDie;
    }

    public void RequestSetCanDie(bool value)
    {
        photonView.RPC("SetCanDieRPC", RpcTarget.MasterClient, value);
    }

    [PunRPC]
    public void SyncSetCanDieRPC(bool value)
    {
        canDie = value;
        OnSetCanDieFeedback?.Invoke(value);
    }

    [PunRPC]
    public void SetCanDieRPC(bool value)
    {
        canDie = value;
        OnSetCanDie?.Invoke(value);
        photonView.RPC("SyncSetCanDieRPC", RpcTarget.All, value);
    }

    public event GlobalDelegates.BoolDelegate OnSetCanDie;
    public event GlobalDelegates.BoolDelegate OnSetCanDieFeedback;

    public void RequestDie()
    {
        photonView.RPC("DieRPC", RpcTarget.MasterClient);
        Debug.Log("Request to die");
    }

    [PunRPC]
    public void SyncDieRPC()
    {
        if (photonView.IsMine)
        {
            InputManager.PlayerMap.Movement.Disable();
            // InputManager.PlayerMap.Attack.Disable();
            InputManager.PlayerMap.Capacity.Disable();
            // InputManager.PlayerMap.Inventory.Disable();
        }

        rotateParent.gameObject.SetActive(false);
        TransformUI.gameObject.SetActive(false);
        // FogOfWarManager.Instance.RemoveFOWViewable(this);

        OnDieFeedback?.Invoke();
    }

    [PunRPC]
    public void DieRPC()
    {
        // TODO : More useful to use that mechanic on decreaseCurrentHp ?
        if (!canDie)
        {
            Debug.LogWarning($"{name} can't die!");
            return;
        }

        isAlive = false;
        OnDie?.Invoke();
        GameStateMachine.Instance.OnTick += Revive;
        photonView.RPC("SyncDieRPC", RpcTarget.All);
    }

    public event GlobalDelegates.NoParameterDelegate OnDie;
    public event GlobalDelegates.NoParameterDelegate OnDieFeedback;

    public void RequestRevive()
    {
        photonView.RPC("ReviveRPC", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void SyncReviveRPC()
    {
        transform.position = respawnPos;
        if (photonView.IsMine)
        {
            InputManager.PlayerMap.Movement.Enable();
            // InputManager.PlayerMap.Attack.Enable();
            InputManager.PlayerMap.Capacity.Enable();
            // InputManager.PlayerMap.Inventory.Enable();
        }

        // FogOfWarManager.Instance.AddFOWViewable(this);
        rotateParent.gameObject.SetActive(true);
        TransformUI.gameObject.SetActive(true);
        OnReviveFeedback?.Invoke();
    }

    [PunRPC]
    public void ReviveRPC()
    {
        isAlive = true;

        SetCurrentHpRPC(maxHp);
        SetCurrentResourceRPC(maxResource);
        OnRevive?.Invoke();
        photonView.RPC("SyncReviveRPC", RpcTarget.All);
    }

    private void Revive()
    {
        respawnTimer += 1 / GameStateMachine.Instance.tickRate;
        if (!(respawnTimer >= respawnDuration)) return;
        GameStateMachine.Instance.OnTick -= Revive;
        respawnTimer = 0f;
        RequestRevive();
    }

    public event GlobalDelegates.NoParameterDelegate OnRevive;
    public event GlobalDelegates.NoParameterDelegate OnReviveFeedback;

    #endregion
}