using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class Entity : MonoBehaviourPun
{
    /// <summary>
    /// The viewID of the photonView of the entity.
    /// </summary>
    public int entityIndex;

    /// <summary>
    /// True if passiveCapacities can be added to the entity's passiveCapacitiesList. False if not.
    /// </summary>
    [SerializeField] private bool canAddPassiveCapacity = true;

    /// <summary>
    /// True if passiveCapacities can be removed from the entity's passiveCapacitiesList. False if not.
    /// </summary>
    [SerializeField] private bool canRemovePassiveCapacity = true;

    /// <summary>
    /// The list of PassiveCapacity on the entity.
    /// </summary>
    public readonly List<PassiveCapacity> passiveCapacitiesList = new List<PassiveCapacity>();

    /// <summary>
    /// The transform of the UI of the entity.
    /// </summary>
    public Transform TransformUI;

    /// <summary>
    /// The offset of the UI of the entity.
    /// </summary>
    public Vector3 OffsetUI = new Vector3(0, 2f, 0);
    
    void Start()
    { 
        entityIndex = photonView.ViewID;
        EntityCollectionManager.AddEntity(this); 
        OnStart();
    }
    protected virtual void OnStart(){}
    
    void Update()
    {
     OnUpdate();   
    }
    protected virtual void OnUpdate(){}
    
    
    public PassiveCapacity GetPassiveCapacityBySOIndex(byte soIndex)
    {
        return passiveCapacitiesList.FirstOrDefault(item => item.indexOfSo == soIndex);
    }
}
